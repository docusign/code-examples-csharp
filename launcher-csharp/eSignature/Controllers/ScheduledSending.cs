using System;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.eSignature.Models;
using DocuSign.CodeExamples.Models;
using eSignature.Examples;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.eSignature.Controllers
{
    [Area("eSignature")]
    [Route("Eg035")]
    public class ScheduledSending : EgController
    {
        public ScheduledSending(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 35;

        public override string EgName => "Eg035";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, DateTime resumeDate)
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

            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Call the Examples API method to create and schedule the envelope
            var envelopeId = global::eSignature.Examples.ScheduledSending.ScheduleEnvelope(signerEmail, signerName, accessToken, basePath, accountId, Config.docPdf, resumeDate);

            RequestItemsService.EnvelopeId = envelopeId;

            ViewBag.h1 = codeExampleText.ResultsPageHeader;
            ViewBag.message = String.Format(codeExampleText.ResultsPageText, envelopeId);
            return View("example_done");
        }
    }
}