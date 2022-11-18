namespace DocuSign.CodeExamples.Click.Controllers
{
    using DocuSign.Click.Client;
    using DocuSign.Click.Examples;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Click")]
    [Route("ClickEg006")]
    public class EmbedClickwrap : EgController
    {
        public EmbedClickwrap(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgName, ExamplesAPIType.Click);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "ClickEg006";

        protected override void InitializeInternal()
        {
            base.InitializeInternal();

            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = RequestItemsService.Session.AccountId;
            ViewBag.ClickwrapsData = DocuSign.Click.Examples.EmbedClickwrap.GetActiveClickwraps(basePath, accessToken, accountId);
            if (ViewBag.ClickwrapsData.Clickwraps.Count == 0)
            {
                ViewBag.InactiveClickwrapsData = DocuSign.Click.Examples.ActivateClickwrap.GetInactiveClickwraps(basePath, accessToken, accountId);
            }
            ViewBag.AccountId = RequestItemsService.Session.AccountId;
        }

        [MustAuthenticate]
        [SetViewBag]
        [Route("Activate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Embed(string clickwrapId, string fullName, string email, string company, string title, string date)
        {

            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = $"{this.RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = this.RequestItemsService.Session.AccountId;

            try
            {

                // Call the Click API to activate a clickwrap
                var clickWrap = DocuSign.Click.Examples.EmbedClickwrap.CreateHasAgreed(clickwrapId, fullName, email, company, title, date, basePath, accessToken, accountId);

                if (clickWrap.AgreementUrl == "Already Agreed")
                {
                    this.ViewBag.errorCode = 200;
                    this.ViewBag.errorMessage = "The email address was already used to agree to this elastic template. Provide a different email address if you want to view the agreement and agree to it.";

                    return this.View("Error");
                }
                else
                {
                    // Show results
                    this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                    this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                    this.ViewBag.agreementURL = clickWrap.AgreementUrl;

                    return this.View("embed");
                }
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;

                return this.View("Error");
            }
        }
    }
}