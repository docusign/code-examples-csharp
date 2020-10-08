using System;
using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using DocuSign.eSign.Client;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg016")]
    public class Eg016SetTabValuesController : EgController
    {
        // Set up the Ping Url, signer client ID, and the return (callback) URL for embedded signing
        private string dsPingUrl;
        private readonly string signerClientId = "1000";
        private string dsReturnUrl;

        public Eg016SetTabValuesController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "SetTabValues";
            dsPingUrl = config.AppUrl + "/";
            dsReturnUrl = config.AppUrl + "/dsReturn";
        }
        public override string EgName => "eg016";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName)
        {
            // Check the token with minimal buffer time
            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            // The envelope will be sent first to the signer; after it is signed,
            // a copy is sent to the cc person
            //
            // Read files from a local directory
            // The reads could raise an exception if the file is not available!
            var basePath = RequestItemsService.Session.BasePath + "/restapi";

            // Step 1: Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Step 2: Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Step 3: Create Tabs and CustomFields
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

            // The salary is set both as a readable number in the /salary/ text field,
            // and as a pure number in a custom field ('salary') in the envelope
            int salary = 123000;

            Text textSalary = new Text
            {
                AnchorString = "/salary/",
                AnchorUnits = "pixels",
                AnchorYOffset = "-9",
                AnchorXOffset = "5",
                Font = "helvetica",
                FontSize = "size11",
                Bold = "true",
                Locked = "true",
                // Convert number to String: 'C2' sets the string 
                // to currency format with two decimal places
                Value = salary.ToString("C2"), 
                TabId = "salary",
                TabLabel = "Salary"
            };

            TextCustomField salaryCustomField = new TextCustomField
            {
                Name = "salary",
                Required = "false",
                Show = "true", // Yes, include in the CoC
                Value = salary.ToString()
            };

            CustomFields cf = new CustomFields
            {
                TextCustomFields = new List<TextCustomField> { salaryCustomField }
            };

            // Create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            Signer signer1 = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                RecipientId = "1",
                RoutingOrder = "1",
                ClientUserId = signerClientId
            };

            // Add the tabs model (including the SignHere tab) to the signer.
            // The Tabs object wants arrays of the different field/tab types
            // Tabs are set per recipient/signer
            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere },
                TextTabs = new List<Text> { textLegal, textFamiliar, textSalary }
            };
            signer1.Tabs = signer1Tabs;
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 }
            };

            string doc1b64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(Config.tabsDocx));

            // Create document objects, one per document
            Document doc1 = new Document
            {
                DocumentBase64 = doc1b64,
                Name = "Lorem Ipsum", // Can be different from actual file name
                FileExtension = "docx",
                DocumentId = "1"
            };

            // Step 4: Create the envelope definition
            EnvelopeDefinition envelopeAttributes = new EnvelopeDefinition()
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
            var envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, envelopeAttributes);
            RequestItemsService.EnvelopeId = results.EnvelopeId;
 
            // Step 6: Create the View Request
            RecipientViewRequest viewRequest = new RecipientViewRequest();
            // Set the URL where you want the recipient to go once they are done signing;
            // this should typically be a callback route somewhere in your app.
            // The query parameter is included as an example of how
            // to save/recover state information during the redirect to
            // the DocuSign signing ceremony. It's usually better to use
            // the session mechanism of your web framework. Query parameters
            // can be changed/spoofed very easily
            viewRequest.ReturnUrl = dsReturnUrl + "?state=123";

            // How has your app authenticated the user? In addition to your app's authentication,
            // you can include authentication steps from DocuSign; e.g., SMS authentication
            viewRequest.AuthenticationMethod = "none";

            // Recipient information must match the embedded recipient info
            // that we used to create the envelope
            viewRequest.Email = signerEmail;
            viewRequest.UserName = signerName;
            viewRequest.ClientUserId = signerClientId;

            // DocuSign recommends that you redirect to DocuSign for the
            // signing ceremony. There are multiple ways to save state.
            // To maintain your application's session, use the PingUrl
            // parameter. It causes the DocuSign Signing Ceremony web page
            // (not the DocuSign server) to send pings via AJAX to your app
            viewRequest.PingFrequency = "600"; // seconds
                                               // NOTE: The pings will only be sent if the pingUrl is an HTTPS address
            viewRequest.PingUrl = dsPingUrl; // Optional setting

            ViewUrl results1 = envelopesApi.CreateRecipientView(accountId, results.EnvelopeId, viewRequest);
            //***********
            // Don't use an iframe with embedded signing requests!
            //***********
            // State can be stored/recovered using the framework's session or a
            // query parameter on the return URL (see the makeRecipientViewRequest method)
            string redirectUrl = results1.Url;
            return Redirect(redirectUrl);
        }
    }
}