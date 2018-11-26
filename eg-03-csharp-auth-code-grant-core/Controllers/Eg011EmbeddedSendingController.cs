using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg011")]
    public class Eg011EmbeddedSendingController : EgController
    {
        private Eg002SigningViaEmailController controller2;

        public Eg011EmbeddedSendingController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            this.controller2 = new Eg002SigningViaEmailController(Config, requestItemsService);
        }

        public override string EgName => "eg011";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName, string startingView)
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
            // Step 1. Make the envelope with "created" (draft) status            
            RequestItemsService.Status = "created";
            //EnvelopeSummary results = (EnvelopeSummary)controller2.doWork(args, model);
            controller2.Create(signerEmail, signerName, ccEmail, ccName);
            String envelopeId = RequestItemsService.EnvelopeId;

            // Step 2. create the sender view
            // Call the CreateSenderView API
            // Exceptions will be caught by the calling function
            string dsReturnUrl = Config.AppUrl + "/ds-return";
            ReturnUrlRequest viewRequest = MakeSenderViewRequest(dsReturnUrl);

            ViewUrl result1 = envelopesApi.CreateSenderView(RequestItemsService.Session.AccountId, envelopeId, viewRequest);
            // Switch to Recipient and Documents view if requested by the user
            String url = result1.Url;
            Console.WriteLine("startingView: " + startingView);
            if ("recipient".Equals(startingView))
            {
                url = url.Replace("send=1", "send=0");
            }

            Console.WriteLine("Sender view URL: " + url);

            return Redirect(url);
        }

        private ReturnUrlRequest MakeSenderViewRequest(string dsReturnUrl)
        {
            ReturnUrlRequest viewRequest = new ReturnUrlRequest();
            // Set the url where you want the recipient to go once they are done signing
            // should typically be a callback route somewhere in your app.
            // The query parameter is included as an example of how
            // to save/recover state information during the redirect to
            // the DocuSign signing ceremony. It's usually better to use
            // the session mechanism of your web framework. Query parameters
            // can be changed/spoofed very easily.
            viewRequest.ReturnUrl = dsReturnUrl + "?state=123";

            return viewRequest;
        }
    }
}