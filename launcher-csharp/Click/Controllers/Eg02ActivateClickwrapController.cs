// <copyright file="Eg02ActivateClickwrapController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

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
    [Route("ClickEg02")]
    public class Eg02ActivateClickwrapController : EgController
    {
        public Eg02ActivateClickwrapController(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgNumber);
            this.ViewBag.title = this.CodeExampleText.PageTitle;
        }

        public override int EgNumber => 2;

        public override string EgName => "ClickEg02";

        protected override void InitializeInternal()
        {
            base.InitializeInternal();

            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = $"{this.RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = this.RequestItemsService.Session.AccountId;
            this.ViewBag.ClickwrapsData = ActivateClickwrap.GetInactiveClickwraps(basePath, accessToken, accountId);
            this.ViewBag.AccountId = this.RequestItemsService.Session.AccountId;
        }

        [MustAuthenticate]
        [Route("Activate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Activate(string clickwrapData)
        {

            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = $"{this.RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = this.RequestItemsService.Session.AccountId;

            try
            {
                string[] Clickwrap = clickwrapData.Split(':');
                var clickwrapId = Clickwrap[0];
                var clickwrapVersion = Clickwrap[1];

                // Call the Click API to activate a clickwrap
                var clickWrap = ActivateClickwrap.Update(clickwrapId, clickwrapVersion, basePath, accessToken, accountId);

                // Show results
                this.ViewBag.h1 = this.CodeExampleText.ResultsPageHeader;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(clickWrap, Formatting.Indented);

                return this.View("example_done");
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