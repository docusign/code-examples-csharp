using DocuSign.Click.Client;
using DocuSign.Click.Examples;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Click.Controllers
{
    [Area("Click")]
    [Route("ClickEg03")]
    public class Eg03CreateNewClickwrapVersionController : EgController
    {
        public Eg03CreateNewClickwrapVersionController(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 3;

        public override string EgName => "ClickEg03";

        protected override void InitializeInternal()
        {
            base.InitializeInternal();

            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = RequestItemsService.Session.AccountId;
            ViewBag.ClickwrapsData = RetrieveClickwraps.GetClickwraps(basePath, accessToken, accountId);
            ViewBag.AccountId = RequestItemsService.Session.AccountId;
        }

        [MustAuthenticate]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string clickwrapId, string clickwrapName)
        {
            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = RequestItemsService.Session.AccountId;

            try
            {
                // Call the Click API to create the clickwrap version
                var clickWrap = CreateNewClickwrapVersion.Create(clickwrapId, basePath, accessToken, accountId, clickwrapName);

                // Show results
                ViewBag.h1 = codeExampleText.ResultsPageHeader;
                ViewBag.message = string.Format(codeExampleText.ResultsPageText, clickWrap.VersionNumber, clickWrap.ClickwrapName);
                ViewBag.Locals.Json = JsonConvert.SerializeObject(clickWrap, Formatting.Indented);

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