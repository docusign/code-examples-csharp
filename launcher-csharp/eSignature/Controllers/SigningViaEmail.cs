using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;
using DocuSign.CodeExamples.eSignature.Models;
using System;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg002")]
    public class SigningViaEmail : EgController
    {
        public SigningViaEmail(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 2;

        public override string EgName => "eg002";

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

            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;

            // Call the Examples API method to create and send an envelope via email
            var envelopeId = global::eSignature.Examples.SigningViaEmail.SendEnvelopeViaEmail(signerEmail, signerName, ccEmail, ccName, accessToken,
                basePath, accountId, Config.docDocx, Config.docPdf, RequestItemsService.Status);

            RequestItemsService.EnvelopeId = envelopeId;

            ViewBag.h1 = codeExampleText.ResultsPageHeader;
            ViewBag.message = String.Format(codeExampleText.ResultsPageHeader, envelopeId);
            return View("example_done");
        }
    }
}