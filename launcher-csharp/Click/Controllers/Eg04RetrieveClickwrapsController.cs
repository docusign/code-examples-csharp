using System.Text;
using DocuSign.Click.Api;
using DocuSign.Click.Client;
using DocuSign.Click.Examples;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.eSignature.Models;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Click.Controllers
{
    [Area("Click")]
    [Route("ClickEg04")]
    public class Eg04RetrieveClickwrapsController : EgController
    {
        public Eg04RetrieveClickwrapsController
            (DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 4;

        public override string EgName => "ClickEg04";
        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            ViewBag.ClickwrapId = RequestItemsService.ClickwrapId;
            ViewBag.AccountId = RequestItemsService.Session.AccountId;
        }
    
    [MustAuthenticate]
        [Route("Retrieve")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Retrieve()
        {
            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = RequestItemsService.Session.AccountId;

            try
            {
                // Call the Click API to get the clickwraps
                var clickwraps = RetrieveClickwraps.GetClickwraps(basePath, accessToken, accountId);

                var messageBuilder = new StringBuilder();
                clickwraps.Clickwraps.ForEach(cw => messageBuilder.AppendLine($"Clickwrap ID:{cw.ClickwrapId}, Clickwrap Version: {cw.VersionNumber}"));

                // Show results
                ViewBag.h1 = codeExampleText.ResultsPageHeader;
                ViewBag.message = codeExampleText.ResultsPageText;
                ViewBag.Locals.Json = JsonConvert.SerializeObject(clickwraps.Clickwraps, Formatting.Indented);

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }
    }
}