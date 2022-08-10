using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;
using DocuSign.eSign.Client;
using DocuSign.CodeExamples.eSignature.Models;

namespace DocuSign.CodeExamples.Views
{
    [Area("eSignature")]
    [Route("Eg039")]
    public class Eg039InPersonSigningController : EgController
    {
        private string DsPingUrl { get; set; }
        private string DsReturnUrl { get; set; }

        public Eg039InPersonSigningController(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            DsPingUrl = config.AppUrl + "/";
            DsReturnUrl = config.AppUrl + "/dsReturn";
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 39;

        public override string EgName => "Eg039";

        [HttpPost]
        public IActionResult Create(string signerName)
        {
            string accessToken = RequestItemsService.User.AccessToken;
            string basePath = RequestItemsService.Session.BasePath + "/restapi";
            string accountId = RequestItemsService.Session.AccountId;
            string hostEmail = RequestItemsService.AuthenticatedUserEmail;
            string hostName = RequestItemsService.User.Name;
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
                    basePath, accountId, Config.docPdf, DsReturnUrl, DsPingUrl);
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