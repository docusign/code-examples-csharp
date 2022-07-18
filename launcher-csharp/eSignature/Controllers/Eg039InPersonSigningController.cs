using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;
using DocuSign.eSign.Client;

namespace DocuSign.CodeExamples.Views
{
    [Area("eSignature")]
    [Route("Eg039")]
    public class Eg039InPersonSigningController : EgController
    {
        private string dsPingUrl;
        private string dsReturnUrl;

        public Eg039InPersonSigningController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            dsPingUrl = config.AppUrl + "/";
            dsReturnUrl = config.AppUrl + "/dsReturn";           
            ViewBag.title = "In Person Signing";
        }

        public override string EgName => "Eg039";

        [HttpPost]
        public IActionResult Create(string hostEmail, string hostName, string signerName)
        {
            string accessToken = RequestItemsService.User.AccessToken;
            string basePath = RequestItemsService.Session.BasePath + "/restapi";
            string accountId = RequestItemsService.Session.AccountId;
            string redirectUrl = string.Empty;

            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            try
            {
                redirectUrl = InPersonSigning.SendEnvelopeForInPersonSigning(hostEmail, hostName, signerName, accessToken, 
                    basePath, accountId, Config.docPdf, dsReturnUrl, dsPingUrl);
            } 
            catch (ApiException apiException) 
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }

            return Redirect(redirectUrl);
        }
    }
}