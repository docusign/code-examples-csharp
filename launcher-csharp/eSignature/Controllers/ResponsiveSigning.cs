using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;
using DocuSign.eSign.Client;
using DocuSign.CodeExamples.eSignature.Models;

namespace DocuSign.CodeExamples.Views
{
    [Area("eSignature")]
    [Route("Eg038")]
    public class ResponsiveSigning : EgController
    {
        private string dsPingUrl;
        private string signerClientId = "1000";
        private string dsReturnUrl;
        private CodeExampleText codeExampleText;

        public ResponsiveSigning(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            dsPingUrl = config.AppUrl + "/";
            dsReturnUrl = config.AppUrl + "/dsReturn";
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 38;

        public override string EgName => "Eg038";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
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
                redirectUrl = global::eSignature.Examples.ResponsiveSigning.CreateEnvelopeFromHTML(signerEmail,
                    signerName, ccEmail, ccName, signerClientId, accessToken, basePath, accountId, dsReturnUrl, dsPingUrl);
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