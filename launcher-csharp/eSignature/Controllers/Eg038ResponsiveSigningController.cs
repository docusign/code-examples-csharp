using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;

namespace DocuSign.CodeExamples.Views
{
    [Area("eSignature")]
    [Route("Eg038")]
    public class Eg038ResponsiveSigningController : EgController
    {
        private string dsPingUrl;
        private string signerClientId = "1000";
        private string dsReturnUrl;

        public Eg038ResponsiveSigningController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {            
            dsPingUrl = config.AppUrl + "/";
            dsReturnUrl = config.AppUrl + "/dsReturn";           
            ViewBag.title = "Embedded Signing Ceremony";
        }

        public override string EgName => "Eg038";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            string accessToken = RequestItemsService.User.AccessToken;
            string basePath = RequestItemsService.Session.BasePath + "/restapi";
            string accountId = RequestItemsService.Session.AccountId;

            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            string redirectUrl = ResponsiveSigning.CreateEnvelopeFromHTML(signerEmail,
                signerName, ccEmail, ccName, signerClientId, accessToken, basePath, accountId, dsReturnUrl, dsPingUrl);

            return Redirect(redirectUrl);
        }
    }
}