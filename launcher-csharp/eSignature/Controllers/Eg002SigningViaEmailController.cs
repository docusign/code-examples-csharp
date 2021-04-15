using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg002")]
    public class Eg002SigningViaEmailController : EgController
    {
        public Eg002SigningViaEmailController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Signing request by email";
        }

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
            var envelopeId = SigningViaEmail.SendEnvelopeViaEmail(signerEmail, signerName, ccEmail, ccName, accessToken,
                basePath, accountId, Config.docDocx, Config.docPdf, RequestItemsService.Status);

            RequestItemsService.EnvelopeId = envelopeId;

            ViewBag.h1 = "Envelope sent";
            ViewBag.message = "The envelope has been created and sent!<br />Envelope ID " + envelopeId + ".";
            return View("example_done");
        }
    }
}