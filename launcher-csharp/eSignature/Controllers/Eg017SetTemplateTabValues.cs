using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg017")]
    public class Eg017SetTemplateTabValuesController : EgController
    {
        // Set up the Ping Url, signer client ID, and the return (callback) URL for embedded signing
        private string dsPingUrl;
        private readonly string signerClientId = "1000";
        private string dsReturnUrl;

        public Eg017SetTemplateTabValuesController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "SetTabValues";
            dsPingUrl = config.AppUrl + "/";
            dsReturnUrl = config.AppUrl + "/dsReturn";
        }

        public override string EgName => "eg017";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Check the token with minimal buffer time
            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            // The envelope will be sent first to the signer; after it is signed,
            // a copy is sent to the cc person
            //
            // Read files from a local directory
            // The reads could raise an exception if the file is not available!
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Call the Examples API method to create an envelope from the template, 
            // setting the tab values in the envelope and generating a signer view
            (string envelopeId, string redirectUrl) = SetTemplateTabValues.CreateEnvelopeFromTempalteAndUpdateTabValues(signerEmail, signerName,
                ccEmail, ccName, signerClientId, accessToken, basePath, accountId, RequestItemsService.TemplateId,
                dsReturnUrl, dsPingUrl);

            RequestItemsService.EnvelopeId = envelopeId;
            return Redirect(redirectUrl);
        }
    }
}
