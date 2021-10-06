using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using eSignature.Examples;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("Eg031")]
    public class Eg031BulkSendEnvelopesController : EgController
    {
        public Eg031BulkSendEnvelopesController(DSConfiguration config, IRequestItemsService requestItemsService) :
            base(config, requestItemsService)
        {
        }

        public override string EgName => "Eg031";


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetProfile(RecipientModel signer1, RecipientModel carbonCopy1, RecipientModel signer2, RecipientModel carbonCopy2)
        {
            // Check the minimal buffer time.
            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            var basePath = RequestItemsService.Session.BasePath + "/restapi";

            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Confirm successful batch send 
                var envelopeIdStamping = "true";
                var emailSubject = "Please sign this document sent from the C# SDK";
                var status = SendBulkEnvelopes.GetStatus(signer1.Name, signer1.Email, carbonCopy1.Name,
                    carbonCopy1.Email, signer2.Name, signer2.Email, carbonCopy2.Name, carbonCopy2.Email, accessToken,
                    basePath, accountId, Config.docPdf, envelopeIdStamping, emailSubject);

                ViewBag.h1 = "Bulk send envelope was successfully performed!";
                ViewBag.message = $@"Bulk request queued to {status.Queued} user lists.";
            }
            catch (Exception ex)
            {
                ViewBag.h1 = "Bulk send envelope failed.";
                ViewBag.message = $@"Bulk request failed to send. Reason: {ex}.";
            }

            return View("example_done");
        }
    }
}
