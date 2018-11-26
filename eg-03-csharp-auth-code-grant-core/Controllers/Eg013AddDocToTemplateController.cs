using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg013")]
    public class Eg013AddDocToTemplateController : EgController
    {
        private string signerClientId = "1000";

        public Eg013AddDocToTemplateController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
        }

        public override string EgName => "eg013";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName, string item, string quantity)
        {
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
            var session = RequestItemsService.Session;
            var user = RequestItemsService.User;
            var config = new Configuration(new ApiClient(session.BasePath + "/restapi"));
            config.AddDefaultHeader("Authorization", "Bearer " + user.AccessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(config);
            string dsReturnUrl = Config.AppUrl + "/dsReturn";
            string dsPingUrl = Config.AppUrl + "/";
            // Step 1. Make the envelope request body
            EnvelopeDefinition envelope = MakeEnvelope(signerEmail, signerName, ccEmail, ccName, item, quantity);
            // Step 2. call Envelopes::create API method
            // Exceptions will be caught by the calling function
            EnvelopeSummary results = envelopesApi.CreateEnvelope(RequestItemsService.Session.AccountId, envelope);

            String envelopeId = results.EnvelopeId;

            Console.WriteLine("Envelope was created. EnvelopeId " + envelopeId);
            // Step 3. create the recipient view, the Signing Ceremony
            RecipientViewRequest viewRequest = MakeRecipientViewRequest(signerEmail, signerName, dsReturnUrl, dsPingUrl);
            ViewUrl results1 = envelopesApi.CreateRecipientView(RequestItemsService.Session.AccountId, envelopeId, viewRequest);
            
            return Redirect(results1.Url);
        }

        private RecipientViewRequest MakeRecipientViewRequest(string signerEmail, string signerName, 
            string dsReturnUrl, string dsPingUrl)
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

        private EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName, string ccEmail, string ccName, string item, string quantity)
        {
            // The envelope request object uses Composite Template to
            // include in the envelope:
            // 1. A template stored on the DocuSign service
            // 2. An additional document which is a custom HTML source document

            // Create Recipients for server template. Note that Recipients object
            // is used, not TemplateRole
            //
            // Create a signer recipient for the signer role of the server template
            Signer signer1 = new Signer();
            signer1.Email = signerEmail;
            signer1.Name = signerName;
            signer1.RoleName = "signer";
            signer1.RecipientId = "1";
            // Adding clientUserId transforms the template recipient
            // into an embedded recipient:
            signer1.ClientUserId = signerClientId;
            // Create the cc recipient
            CarbonCopy cc1 = new CarbonCopy();
            cc1.Email = ccEmail;
            cc1.Name = ccName;
            cc1.RoleName = "cc";
            cc1.RecipientId = "2";
            // Recipients object:
            Recipients recipientsServerTemplate = new Recipients();
            recipientsServerTemplate.CarbonCopies = new List<CarbonCopy> { cc1 };
            recipientsServerTemplate.Signers = new List<Signer> { signer1 };

            // create a composite template for the Server Template
            CompositeTemplate compTemplate1 = new CompositeTemplate();
            compTemplate1.CompositeTemplateId  = "1";
            ServerTemplate serverTemplates = new ServerTemplate();
            serverTemplates.Sequence = "1";
            serverTemplates.TemplateId = RequestItemsService.TemplateId;

            compTemplate1.ServerTemplates = new List<ServerTemplate> { serverTemplates };
            // Add the roles via an inlineTemplate
            InlineTemplate inlineTemplate = new InlineTemplate();
            inlineTemplate.Sequence = "1";
            inlineTemplate.Recipients = recipientsServerTemplate;
            compTemplate1.InlineTemplates = new List<InlineTemplate> { inlineTemplate };
            // The signer recipient for the added document with
            // a tab definition:
            SignHere signHere1 = new SignHere();
            signHere1.AnchorString = "**signature_1**";
            signHere1.AnchorYOffset = "10";
            signHere1.AnchorUnits = "pixels";
            signHere1.AnchorXOffset = "20";

            Tabs signer1Tabs = new Tabs();
            signer1Tabs.SignHereTabs = new List<SignHere> { signHere1 };
            // Signer definition for the added document
            Signer signer1AddedDoc = new Signer();
            signer1AddedDoc.Email = signerEmail;
            signer1AddedDoc.Name = signerName;
            signer1AddedDoc.ClientUserId = signerClientId;
            signer1AddedDoc.RoleName = "signer";
            signer1AddedDoc.RecipientId = "1";
            signer1AddedDoc.Tabs =   signer1Tabs;
            // Recipients object for the added document:
            Recipients recipientsAddedDoc = new Recipients();
            recipientsAddedDoc.CarbonCopies = new List<CarbonCopy> { cc1 };
            recipientsAddedDoc.Signers = new List<Signer> { signer1AddedDoc };

            // create the HTML document
            Document doc1 = new Document();
            
            String doc1b64 = Convert.ToBase64String(document1(signerEmail, signerName, ccEmail, ccName, item, quantity));
            doc1.DocumentBase64 = doc1b64;
            doc1.Name = "Appendix 1--Sales order"; // can be different from actual file name
            doc1.FileExtension = "html";
            doc1.DocumentId = "1";
            // create a composite template for the added document
            CompositeTemplate compTemplate2 = new CompositeTemplate();
            compTemplate2.CompositeTemplateId = "2";
            // Add the recipients via an inlineTemplate
            InlineTemplate inlineTemplate2 = new InlineTemplate();
            inlineTemplate2.Sequence = "2";
            inlineTemplate2.Recipients = recipientsAddedDoc;
            compTemplate2.InlineTemplates = new List<InlineTemplate> { inlineTemplate2};
            compTemplate2.Document = doc1;

            EnvelopeDefinition env = new EnvelopeDefinition();
            env.Status = "sent";
            env.CompositeTemplates = new List<CompositeTemplate> { compTemplate1, compTemplate2};

            return env;
        }

        private byte[] document1(string signerEmail, string signerName, string ccEmail, string ccName, 
            string item, string quantity)
        {
            return Encoding.UTF8.GetBytes(" <!DOCTYPE html>\n" +
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
                    "        <p style=\"margin-top:0em; margin-bottom:0em;\">Copy to: " + ccName + "," + ccEmail + "</p>\n" +
                    "        <p style=\"margin-top:3em; margin-bottom:0em;\">Item: <b>" + item + "</b>, quantity: <b>" + quantity + "</b> at market price.</p>\n" +
                    "        <p style=\"margin-top:3em;\">\n" +
                    "  Candy bonbon pastry jujubes lollipop wafer biscuit biscuit. Topping brownie sesame snaps sweet roll pie. Croissant danish biscuit soufflé caramels jujubes jelly. Dragée danish caramels lemon drops dragée. Gummi bears cupcake biscuit tiramisu sugar plum pastry. Dragée gummies applicake pudding liquorice. Donut jujubes oat cake jelly-o. Dessert bear claw chocolate cake gummies lollipop sugar plum ice cream gummies cheesecake.\n" +
                    "        </p>\n" +
                    "        <!-- Note the anchor tag for the signature field is in white. -->\n" +
                    "        <h3 style=\"margin-top:3em;\">Agreed: <span style=\"color:white;\">**signature_1**/</span></h3>\n" +
                    "        </body>\n" +
                    "    </html>");
        }
    }
}