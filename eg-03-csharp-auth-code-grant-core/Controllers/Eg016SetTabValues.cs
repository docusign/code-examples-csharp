using System;
using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using DocuSign.eSign.Client;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg016")]
    public class Eg016SetTabValuesController : EgController
    {
        public Eg016SetTabValuesController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "SetTabValues";
        }

        public override string EgName => "eg016";


        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName)
        {
            // Check the token with minimal buffer time.
            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }


            // The salary is set both as a readable number in the
            // /salary/ text field, and as a pure number in a
            // custom field ('salary') in the envelope.

            // The envelope will be sent first to the signer.
            // After it is signed, a copy is sent to the cc person.
            // read files from a local directory
            // The reads could raise an exception if the file is not available!

            var basePath = RequestItemsService.Session.BasePath + "/restapi";

            // Step 1: Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; //represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; //represents your {ACCOUNT_ID}

            // Step 2: Construct your API headers
            var config = new Configuration(new ApiClient(basePath));
            config.AddDefaultHeader("Authorization", "Bearer " + accessToken);

            // Step 3: Construct your envelope JSON body
            // Salary that will be used.
            int salary = 123000;

      
            string doc1b64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(Config.tabsDocx));


            // Create document objects, one per document

            Document doc1 = new Document
            {
                DocumentBase64 = doc1b64,
                Name = "Lorem Ipsum", // can be different from actual file name
                FileExtension = "docx",
                DocumentId = "1"
            };


            // The order in the docs array determines the order in the envelope
            

            // create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            Signer signer1 = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                RecipientId = "1",
                RoutingOrder = "1",
                //ClientUserId = "1000" // The id of the signer within this application.
            };

            // Create signHere fields (also known as tabs) on the documents,
            // We're using anchor (autoPlace) positioning
            //
            // The DocuSign platform searches throughout your envelope's
            // documents for matching anchor strings. So the
            // signHere2 tab will be used in both document 2 and 3 since they
            // use the same anchor string for their "signer 1" tabs.
            SignHere signHere = new SignHere
            {
                AnchorString = "/sn1/",
                AnchorUnits = "pixels",
                AnchorYOffset = "10",
                AnchorXOffset = "20"
            };

            Text textLegal = new Text
            {
                AnchorString = "/legal/",
                AnchorUnits = "pixels",
                AnchorYOffset = "-9",
                AnchorXOffset = "5",
                Font = "helvetica",
                FontSize = "size11",
                Bold = "true",
                Value = signerName,
                Locked = "false",
                TabId = "legal_name",
                TabLabel = "Legal name",
            };

            Text textFamiliar = new Text
            {
                AnchorString = "/familiar/",
                AnchorUnits = "pixels",
                AnchorYOffset = "-9",
                AnchorXOffset = "5",
                Font = "helvetica",
                FontSize = "size11",
                Bold = "true",
                Value = signerName,
                Locked = "false",
                TabId = "familiar_name",
                TabLabel = "Familiar name"
            };

            Text textSalary = new Text
            {
                AnchorString = "/salary/",
                AnchorUnits = "pixels",
                AnchorYOffset = "-9",
                AnchorXOffset = "5",
                Font = "helvetica",
                FontSize = "size11",
                Bold = "true",
                Value = salary.ToString("C2"),
                Locked = "true",
                TabId = "salary",
                TabLabel = "Salary"
            };

            // The SDK can't create a number tab at this time. (not confirmed) Bug DCM-2732
            TextCustomField salaryCustomField = new TextCustomField
            {
                Name = "salary",
                Required = "false",
                Show = "true", // Yes, include in the CoC
                Value = salary.ToString()


            };
            // Step 4: Create a Custom Tabs Object
            CustomFields cf = new CustomFields
            {
                TextCustomFields = new List<TextCustomField> { salaryCustomField }
            };


            // Add the tabs model (including the SignHere tab) to the signer.
            // The Tabs object wants arrays of the different field/tab types
            // Tabs are set per recipient / signer
            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere },
                TextTabs = new List<Text> { textLegal, textFamiliar, textSalary },
            };
            signer1.Tabs = signer1Tabs;

            // Add the recipients to the envelope object
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 }
            };

            // create the envelope definition.
            EnvelopeDefinition env = new EnvelopeDefinition()
            {

                EnvelopeIdStamping = "true",
                EmailSubject = "Please Sign",
                EmailBlurb = "Sample text for email body",
                Status = "Sent",
                Recipients = recipients,
                CustomFields = cf,
                Documents = new List<Document> { doc1 }

            };

            // Step 5: Call the eSignature REST API
            EnvelopesApi envelopesApi = new EnvelopesApi(config);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, env);

            ViewBag.h1 = "Envelope sent";
            ViewBag.message = "The envelope has been created and sent!<br />Envelope ID " + results.EnvelopeId + ".";
            return View("example_done");
        }
    }
}