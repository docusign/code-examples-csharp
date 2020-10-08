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
    [Route("eg002")]
    public class Eg002SigningViaEmailController : EgController
    {
        public Eg002SigningViaEmailController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Signing request by email";
        }

        public override string EgName => "eg002";

        // ***DS.snippet.0.start
        public EnvelopeSummary DoWork(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;

            EnvelopeDefinition env = MakeEnvelope(signerEmail, signerName, ccEmail, ccName);
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, env);
            RequestItemsService.EnvelopeId = results.EnvelopeId;
            return results;
        }

        private EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Data for this method
            // signerEmail
            // signerName
            // ccEmail
            // ccName
            // Config.docDocx
            // Config.docPdf
            // RequestItemsService.Status -- the envelope status ('created' or 'sent')


            // document 1 (html) has tag **signature_1**
            // document 2 (docx) has tag /sn1/
            // document 3 (pdf) has tag /sn1/
            //
            // The envelope has two recipients.
            // recipient 1 - signer
            // recipient 2 - cc
            // The envelope will be sent first to the signer.
            // After it is signed, a copy is sent to the cc person.
            // read files from a local directory
            // The reads could raise an exception if the file is not available!
            string doc2DocxBytes = Convert.ToBase64String(System.IO.File.ReadAllBytes(Config.docDocx));
            string doc3PdfBytes = Convert.ToBase64String(System.IO.File.ReadAllBytes(Config.docPdf)); 
            // create the envelope definition
            EnvelopeDefinition env = new EnvelopeDefinition();
            env.EmailSubject = "Please sign this document set";

            // Create document objects, one per document
            Document doc1 = new Document();
            string b64 = Convert.ToBase64String(document1(signerEmail, signerName, ccEmail, ccName));
            doc1.DocumentBase64 = b64;
            doc1.Name = "Order acknowledgement"; // can be different from actual file name
            doc1.FileExtension = "html"; // Source data format. Signed docs are always pdf.
            doc1.DocumentId = "1"; // a label used to reference the doc
            Document doc2 = new Document {
                DocumentBase64 = doc2DocxBytes,
                Name = "Battle Plan", // can be different from actual file name
                FileExtension = "docx",
                DocumentId = "2"
            };
            Document doc3 = new Document
            {
                DocumentBase64 = doc3PdfBytes,
                Name = "Lorem Ipsum", // can be different from actual file name
                FileExtension = "pdf",
                DocumentId = "3"
            };
            // The order in the docs array determines the order in the envelope
            env.Documents =  new List<Document> { doc1, doc2, doc3};

            // create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            Signer signer1 = new Signer {
                Email = signerEmail,
                Name = signerName,
                RecipientId = "1",
                RoutingOrder = "1"
            };

            // routingOrder (lower means earlier) determines the order of deliveries
            // to the recipients. Parallel routing order is supported by using the
            // same integer as the order for two or more recipients.

            // create a cc recipient to receive a copy of the documents, identified by name and email
            // We're setting the parameters via setters
            CarbonCopy cc1 = new CarbonCopy
            {
                Email = ccEmail,
                Name = ccName,
                RecipientId = "2",
                RoutingOrder = "2"
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
            Tabs signer1Tabs = new Tabs {
                SignHereTabs = new List<SignHere> { signHere1, signHere2}
            };
            signer1.Tabs = signer1Tabs;

            // Add the recipients to the envelope object
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 },
                CarbonCopies = new List<CarbonCopy> { cc1 }
            };
            env.Recipients = recipients;
            // Request that the envelope be sent by setting |status| to "sent".
            // To request that the envelope be created as a draft, set to "created"
            env.Status = RequestItemsService.Status;

            return env;
        }

        private byte[] document1(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Data for this method
            // signerEmail
            // signerName
            // ccEmail
            // ccName

            return Encoding.UTF8.GetBytes(
            " <!DOCTYPE html>\n" +
                "    <html>\n" +
                "        <head>\n" +
                "          <meta charset=\"UTF-8\">\n" +
                "        </head>\n" +
                "        <body style=\"font-family:sans-serif;margin-left:2em;\">\n" +
                "        <h1 style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\n" +
                "            color: darkblue;margin-bottom: 0;\">World Wide Corp</h1>\n" +
                "        <h2 style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\n" +
                "          margin-top: 0px;margin-bottom: 3.5em;font-size: 1em;\n" +
                "          color: darkblue;\">Order Processing Division</h2>\n" +
                "        <h4>Ordered by " + signerName + "</h4>\n" +
                "        <p style=\"margin-top:0em; margin-bottom:0em;\">Email: " + signerEmail + "</p>\n" +
                "        <p style=\"margin-top:0em; margin-bottom:0em;\">Copy to: " + ccName + ", " + ccEmail + "</p>\n" +
                "        <p style=\"margin-top:3em;\">\n" +
                "  Candy bonbon pastry jujubes lollipop wafer biscuit biscuit. Topping brownie sesame snaps sweet roll pie. Croissant danish biscuit soufflé caramels jujubes jelly. Dragée danish caramels lemon drops dragée. Gummi bears cupcake biscuit tiramisu sugar plum pastry. Dragée gummies applicake pudding liquorice. Donut jujubes oat cake jelly-o. Dessert bear claw chocolate cake gummies lollipop sugar plum ice cream gummies cheesecake.\n" +
                "        </p>\n" +
                "        <!-- Note the anchor tag for the signature field is in white. -->\n" +
                "        <h3 style=\"margin-top:3em;\">Agreed: <span style=\"color:white;\">**signature_1**/</span></h3>\n" +
                "        </body>\n" +
                "    </html>"
                );
        }
        // ***DS.snippet.0.end

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Check the token with minimal buffer time.
            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation 
                // so it could be restarted automatically.
                // But since it should be rare to have a token issue here,
                // we'll make the user re-enter the form data after 
                // authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }
            EnvelopeSummary results = DoWork(signerEmail, signerName, ccEmail, ccName);
            ViewBag.h1 = "Envelope sent";
            ViewBag.message = "The envelope has been created and sent!<br />Envelope ID " + results.EnvelopeId + ".";
            return View("example_done");
        }
    }
}