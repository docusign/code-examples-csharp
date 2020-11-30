using System;
using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg029")]
    public class Eg029ApplyBrandToEnvelopeController : EgController
    {
        public Eg029ApplyBrandToEnvelopeController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "Apply a brand to an envelope";
        }

        public override string EgName => "eg029";

        protected override void InitializeInternal()
        {
            // Data for this method
            // signerEmail 
            // signerName
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var accountsApi = new AccountsApi(apiClient);
            var brands = accountsApi.ListBrands(accountId);

            ViewBag.Brands = brands.Brands;
        }

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string brandId)
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

            // Data for this method
            // signerEmail 
            // signerName
            var basePath = RequestItemsService.Session.BasePath + "/restapi";

            // Step 1. Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Step 2. Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Step 3. Construct your request body
            EnvelopeDefinition env = MakeEnvelope(signerEmail, signerName, brandId);

            // Step 4. Call the eSignature REST API
            var envelopesApi = new EnvelopesApi(apiClient);
            var results = envelopesApi.CreateEnvelope(accountId, env);
            
            ViewBag.h1 = "Envelope sent";
            ViewBag.message = "The envelope has been created and sent!<br />Envelope ID " + results.EnvelopeId + ".";
            return View("example_done");
        }

        private EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName, string brandId)
        {
            // Data for this method
            // signerEmail
            // signerName
            // ccEmail
            // ccName
            // Config.docDocx
            // Config.docPdf
            // RequestItemsService.Status -- the envelope status ('created' or 'sent')


            // The envelope has two recipients.
            // recipient 1 - signer
            // The envelope will be sent first to the signer.
            // After it is signed, a copy is sent to the cc person.
            // read files from a local directory
            // The reads could raise an exception if the file is not available!
            string docPdfBytes = Convert.ToBase64String(System.IO.File.ReadAllBytes(Config.docPdf));
            // create the envelope definition
            EnvelopeDefinition env = new EnvelopeDefinition();
            env.EmailSubject = "Please sign this document set";

            // Create document objects, one per document
            Document doc = new Document
            {
                DocumentBase64 = docPdfBytes,
                Name = "Lorem Ipsum", // can be different from actual file name
                FileExtension = "pdf",
                DocumentId = "1"
            };
            // The order in the docs array determines the order in the envelope
            env.Documents = new List<Document> { doc };

            // create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            Signer signer1 = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                RecipientId = "1",
                RoutingOrder = "1"
            };


            // Create signHere fields (also known as tabs) on the documents,
            // We're using anchor (autoPlace) positioning
            //
            // The DocuSign platform searches throughout your envelope's
            // documents for matching anchor strings. So the
            // signHere2 tab will be used in both document 2 and 3 since they
            // use the same anchor string for their "signer 1" tabs.
            SignHere signHere1 = new SignHere
            {
                AnchorString = "**signature_1**",
                AnchorUnits = "pixels",
                AnchorYOffset = "10",
                AnchorXOffset = "20"
            };

            SignHere signHere2 = new SignHere
            {
                AnchorString = "/sn1/",
                AnchorUnits = "pixels",
                AnchorYOffset = "10",
                AnchorXOffset = "20"
            };

            // Tabs are set per recipient / signer
            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere1, signHere2 }
            };
            signer1.Tabs = signer1Tabs;

            // Add the recipients to the envelope object
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 }
            };
            env.Recipients = recipients;
            // Request that the envelope be sent by setting |status| to "sent".
            // To request that the envelope be created as a draft, set to "created"
            env.Status = RequestItemsService.Status;

            //Set the brand id.
            env.BrandId = brandId;

            return env;
        }
    }
}