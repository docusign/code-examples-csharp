using System;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg011")]
    public class Eg011EmbeddedSendingController : EgController
    {
        public Eg011EmbeddedSendingController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Embedded Sending";
        }

        public override string EgName => "eg011";

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

            // Call the Examples API method to create the envelope and send it using embedded sending
            var redirectUrl = EmbeddedSending.SendEnvelopeUsingEmbeddedSending(signerEmail, signerName, ccEmail, ccName,
                Config.docDocx, Config.docPdf, accessToken, basePath, accountId, startingView, dsReturnUrl);

            Console.WriteLine("Sender view URL: " + redirectUrl);
            return Redirect(redirectUrl);
        }
    }
}