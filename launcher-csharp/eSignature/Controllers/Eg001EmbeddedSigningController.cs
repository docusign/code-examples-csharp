using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;

namespace DocuSign.CodeExamples.Views
{
    [Route("eg001")]
    public class Eg001EmbeddedSigningController : EgController
    {
        private string dsPingUrl;
        private string signerClientId = "1000";
        private string dsReturnUrl;

        public Eg001EmbeddedSigningController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {            
            dsPingUrl = config.AppUrl + "/";
            dsReturnUrl = config.AppUrl + "/dsReturn";           
            ViewBag.title = "Embedded Signing Ceremony";
        }

        public override string EgName => "eg001";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // dsPingUrl -- class global
            // signerClientId -- class global
            // dsReturnUrl -- class global
            string accessToken = RequestItemsService.User.AccessToken;
            string basePath = RequestItemsService.Session.BasePath + "/restapi";
            string accountId = RequestItemsService.Session.AccountId;

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

            // Call the method from Examples API to send envelope and generate url for embedded signing
            var result = EmbeddedSigningCeremony.SendEnvelopeForEmbeddedSigning(signerEmail,
                signerName, signerClientId, accessToken, basePath, accountId, Config.docPdf, dsReturnUrl, dsPingUrl);

            // Save for future use within the example launcher
            RequestItemsService.EnvelopeId = result.Item1;

            // Redirect the user to the Signing Ceremony
            return Redirect(result.Item2);
        }
    }
}