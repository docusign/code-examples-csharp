using System;
using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using eg_03_csharp_auth_code_grant_core.Common;
using eg_03_csharp_auth_code_grant_core.Controllers;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Views
{
    [Route("eg001")]
    public class Eg001EmbeddedSigningController : EgController
    {
        private string dsPingUrl;
        private string signerClientId = "1000";
        private string dsReturnUrl;

        public Eg001EmbeddedSigningController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {            
            dsPingUrl = config.AppUrl + "/";
            dsReturnUrl = config.AppUrl + "/dsReturn";           
            ViewBag.title = "Embedded Signing Ceremony";
        }

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName)
        {
            var session = RequestItemsService.Session;
            var user = RequestItemsService.User;
            // Step 1. Create the envelope definition
            EnvelopeDefinition envelope = MakeEnvelope(signerEmail, signerName);

            // Step 2. Call DocuSign to create the envelope
            //EnvelopesApi envelopesApi = new EnvelopesApi((Configuration)HttpContext.Items["docuSignConfig"]);
            EnvelopesApi envelopesApi = new EnvelopesApi(RequestItemsService.DefaultConfiguration);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(session.AccountId, envelope);

            string envelopeId = results.EnvelopeId;

            // Save for future use within the example launcher
            RequestItemsService.EnvelopeId = envelopeId;

            // Step 3. create the recipient view, the Signing Ceremony
            RecipientViewRequest viewRequest = MakeRecipientViewRequest(signerEmail, signerName);
            // call the CreateRecipientView API
            ViewUrl results1 = envelopesApi.CreateRecipientView(session.AccountId, envelopeId, viewRequest);

            // Step 4. Redirect the user to the Signing Ceremony
            // Don't use an iFrame!
            // State can be stored/recovered using the framework's session or a
            // query parameter on the returnUrl (see the makeRecipientViewRequest method)
            return Redirect(results1.Url);
        }

        private RecipientViewRequest MakeRecipientViewRequest(string signerEmail, string signerName)
        {
            RecipientViewRequest viewRequest = new RecipientViewRequest();
            // Set the url where you want the recipient to go once they are done signing
            // should typically be a callback route somewhere in your app.
            // The query parameter is included as an example of how
            // to save/recover state information during the redirect to
            // the DocuSign signing ceremony. It's usually better to use
            // the session mechanism of your web framework. Query parameters
            // can be changed/spoofed very easily.
            viewRequest.ReturnUrl = dsReturnUrl + "?state=123";

            // How has your app authenticated the user? In addition to your app's
            // authentication, you can include authenticate steps from DocuSign.
            // Eg, SMS authentication
            viewRequest.AuthenticationMethod = "none";

            // Recipient information must match embedded recipient info
            // we used to create the envelope.
            viewRequest.Email = signerEmail;
            viewRequest.UserName = signerName;
            viewRequest.ClientUserId = signerClientId;

            // DocuSign recommends that you redirect to DocuSign for the
            // Signing Ceremony. There are multiple ways to save state.
            // To maintain your application's session, use the pingUrl
            // parameter. It causes the DocuSign Signing Ceremony web page
            // (not the DocuSign server) to send pings via AJAX to your
            // app,
            viewRequest.PingFrequency = "600"; // seconds
                                               // NOTE: The pings will only be sent if the pingUrl is an https address
            viewRequest.PingUrl = dsPingUrl; // optional setting

            return viewRequest;
        }

        private EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName)
        {
            byte[] buffer = System.IO.File.ReadAllBytes(Config.docPdf);

            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition();
            envelopeDefinition.EmailSubject = "Please sign this document";
            Document doc1 = new Document();

            String doc1b64 = Convert.ToBase64String(buffer);

            doc1.DocumentBase64 = doc1b64;
            doc1.Name = "Lorem Ipsum"; // can be different from actual file name
            doc1.FileExtension = "pdf";
            doc1.DocumentId = "3";

            // The order in the docs array determines the order in the envelope
            envelopeDefinition.Documents = new List<Document> { doc1 };

            // Create a signer recipient to sign the document, identified by name and email
            // We set the clientUserId to enable embedded signing for the recipient
            // We're setting the parameters via the object creation
            Signer signer1 = new Signer {
                Email = signerEmail,
                Name = signerName,
                ClientUserId = signerClientId,
                RecipientId = "1"
            };
           
            // Create signHere fields (also known as tabs) on the documents,
            // We're using anchor (autoPlace) positioning
            //
            // The DocuSign platform seaches throughout your envelope's
            // documents for matching anchor strings.
            SignHere signHere1 = new SignHere
            {
                AnchorString = "/sn1/",
                AnchorUnits = "pixels",
                AnchorXOffset = "10",
                AnchorYOffset = "20"
            };
            // Tabs are set per recipient / signer
            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere1 }
            };
            signer1.Tabs = signer1Tabs;

            // Add the recipient to the envelope object
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 }
            };
            envelopeDefinition.Recipients = recipients;

            // Request that the envelope be sent by setting |status| to "sent".
            // To request that the envelope be created as a draft, set to "created"
            envelopeDefinition.Status = "sent";

            return envelopeDefinition;
        }

        public override string EgName => "eg001";
    }
}