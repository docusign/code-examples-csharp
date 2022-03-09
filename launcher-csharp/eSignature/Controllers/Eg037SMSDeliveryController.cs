using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using eSignature.Examples;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.eSignature.Controllers
{
    [Area("eSignature")]
    [Route("Eg037")]
    public class Eg037SMSDeliveryController : EgController
    {
        public Eg037SMSDeliveryController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
        }

        public override string EgName => "Eg037";

        [HttpPost]
        public IActionResult Create(string signerName, string signerCountryCode, string signerPhoneNumber, string ccName, string ccCountryCode, string ccPhoneNumber)
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
            var envelopeId = SMSDelivery.SendRequestViaSMS(accessToken, basePath, accountId, signerName,
                signerCountryCode, signerPhoneNumber, ccName, ccCountryCode, ccPhoneNumber, Config.docDocx,
                Config.docPdf, RequestItemsService.Status);

            RequestItemsService.EnvelopeId = envelopeId;

            ViewBag.h1 = "Envelope sent";
            ViewBag.message = "The envelope has been created and sent!<br />Envelope ID " + envelopeId + ".";
            return View("example_done");
        }
    }
}