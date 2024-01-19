// <copyright file="ActivateClickwrap.cs" company="DocuSign">
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
    [Route("ceg002")]
    public class ActivateClickwrap : EgController
    {
        public ActivateClickwrap(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Click);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "ceg002";

        [MustAuthenticate]
        [SetViewBag]
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
                string[] clickwrap = clickwrapData.Split(':');
                var clickwrapId = clickwrap[0];
                var clickwrapVersion = clickwrap[1];

                // Call the Click API to activate a clickwrap
                var clickWrap = DocuSign.Click.Examples.ActivateClickwrap.Update(clickwrapId, clickwrapVersion, basePath, accessToken, accountId);

                // Show results
                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(clickWrap, Formatting.Indented);

                return this.View("example_done");
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }
        }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();

            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = $"{this.RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = this.RequestItemsService.Session.AccountId;

            var inactiveClickwraps = DocuSign.Click.Examples.ActivateClickwrap.GetClickwrapsByStatus(basePath, accessToken, accountId, "inactive");
            var draftClickwraps = DocuSign.Click.Examples.ActivateClickwrap.GetClickwrapsByStatus(basePath, accessToken, accountId, "draft");
            inactiveClickwraps.Clickwraps.AddRange(draftClickwraps.Clickwraps);

            this.ViewBag.ClickwrapsData = inactiveClickwraps;
            this.ViewBag.AccountId = this.RequestItemsService.Session.AccountId;
        }
    }
}