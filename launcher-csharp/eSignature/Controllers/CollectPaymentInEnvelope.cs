using System;
using System.Collections.Generic;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;
using DocuSign.CodeExamples.eSignature.Models;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg014")]
    public class CollectPaymentInEnvelope : EgController
    {
        public CollectPaymentInEnvelope(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 14;

        public override string EgName => "eg014";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;

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

            // Call the Examples API method to create an envelope with payment processing
            string envelopeId = global::eSignature.Examples.CollectPaymentInEnvelope.CreateEnvelopeWithPayment(signerEmail, signerName, ccEmail,
                ccName, accessToken, basePath, accountId, RequestItemsService.Status, Config.GatewayAccountId, Config.GatewayName, Config.GatewayDisplayName);

            // Process results
            Console.WriteLine("Envelope was created.EnvelopeId " + envelopeId);
            ViewBag.h1 = codeExampleText.ResultsPageHeader;
            ViewBag.message = String.Format(codeExampleText.ResultsPageHeader, envelopeId);
            return View("example_done");
        }
    }
}
