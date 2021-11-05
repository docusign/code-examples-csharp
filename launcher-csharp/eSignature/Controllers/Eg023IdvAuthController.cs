using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;
using DocuSign.eSign.Client;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg023")]
    public class Eg023IdvAuthController : EgController
    {
        public Eg023IdvAuthController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "ID Verification Authentication";
        }

        public override string EgName => "eg023";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            try
            {
                // Check the token with minimal buffer time.
                bool tokenOk = CheckToken(3);
                if (!tokenOk)
                {
                    // We could store the parameters of the requested operation so it could be 
                    // restarted automatically. But since it should be rare to have a token issue
                    // here, we'll make the user re-enter the form data after authentication.
                    RequestItemsService.EgName = EgName;
                    return Redirect("/ds/mustAuthenticate");
                }

                // Data for this method
                // signerEmail 
                // signerName
                var basePath = RequestItemsService.Session.BasePath + "/restapi";

                // Obtain your OAuth token
                var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
                var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

                // Call the Examples API method to create an envelope and 
                // add recipient that is to be authenticated with IDV
                string envelopeId = RecipientAuthIDV.CreateEnvelopeWithRecipientUsingIDVAuth(signerEmail, signerName, accessToken, basePath, accountId);

                // Process results
                ViewBag.h1 = "Envelope sent";
                ViewBag.message = "The envelope has been created and sent!<br />Envelope ID " + envelopeId + ".";
                return View("example_done");
            }
            catch (ApiException apiException)
            {
                if (apiException.Message.Contains("IDENTITY_WORKFLOW_INVALID_ID"))
                {
                    // This may indicate that this account is not yet enabled for the new phone auth workflow
                    ViewBag.SupportMessage = "Please contact <a target='_blank' href='https://support.docusign.com'>Support</a> to enable IDV in your account.";
                }
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }
    }
}