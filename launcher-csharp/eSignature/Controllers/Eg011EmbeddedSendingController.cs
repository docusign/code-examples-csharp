using System;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg011")]
    public class Eg011EmbeddedSendingController : EgController
    {
        private Eg002SigningViaEmailController controller2;

        public Eg011EmbeddedSendingController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Embedded Sending";
        }

        public override string EgName => "eg011";

        // ***DS.snippet.0.start
        private string DoWork(string signerEmail, string signerName, string ccEmail,
            string ccName, string accessToken, string basePath,
            string accountId, string startingView, string dsReturnUrl)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName
            // startingView
            // accessToken
            // basePath 
            // accountId 
            // dsReturnUrl

            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var envelopesApi = new EnvelopesApi(apiClient);

            // Step 1. Make the envelope with "created" (draft) status            
            // Using eg002 to create the envelope with "created" status
            RequestItemsService.Status = "created";
            controller2 = new Eg002SigningViaEmailController(Config, RequestItemsService);
            EnvelopeSummary results = controller2.DoWork(signerEmail, signerName, ccEmail, ccName);
            String envelopeId = results.EnvelopeId;

            // Step 2. create the sender view
            // Call the CreateSenderView API
            // Exceptions will be caught by the calling function
            ReturnUrlRequest viewRequest = new ReturnUrlRequest
            {
                ReturnUrl = dsReturnUrl
            };
            ViewUrl result1 = envelopesApi.CreateSenderView(accountId, envelopeId, viewRequest);
            // Switch to Recipient and Documents view if requested by the user
            String redirectUrl = result1.Url;
            Console.WriteLine("startingView: " + startingView);
            if ("recipient".Equals(startingView))
            {
                redirectUrl = redirectUrl.Replace("send=1", "send=0");
            }
            return redirectUrl;
        }
        // ***DS.snippet.0.end

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName, string startingView)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName
            // startingView
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;
            string dsReturnUrl = Config.AppUrl + "/dsReturn";

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

            string redirectUrl = DoWork(signerEmail,  signerName,  ccEmail,
                ccName,  accessToken,  basePath,
                accountId,  startingView,  dsReturnUrl);

            Console.WriteLine("Sender view URL: " + redirectUrl);
            return Redirect(redirectUrl);
        }
    }
}