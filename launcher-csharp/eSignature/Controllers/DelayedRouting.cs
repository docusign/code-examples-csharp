using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using eSignature.Examples;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.eSignature.Controllers
{
    [Area("eSignature")]
    [Route("Eg036")]
    public class DelayedRouting : EgController
    {
        public DelayedRouting(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
        }

        public override string EgName => "Eg036";

        [HttpPost]
        public IActionResult Create(string signer1Email, string signer1Name, string signer2Email, string signer2Name, int delay)
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

            // Call the Examples API method to create and send an envelope and notify recipients via SMS
            var envelopeId = global::eSignature.Examples.DelayedRouting.SendEnvelopeWithDelayedRouting(signer1Email, signer1Name, signer2Email, signer2Name, accessToken, basePath, accountId, Config.docPdf, delay);

            RequestItemsService.EnvelopeId = envelopeId;

            ViewBag.h1 = "Envelope sent";
            ViewBag.message = "The envelope has been created and sent!<br />Envelope ID " + envelopeId + ".";
            return View("example_done");
        }
    }
}