// <copyright file="CreateNewClickwrapVersion.cs" company="DocuSign">
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
    [Route("ClickEg003")]
    public class CreateNewClickwrapVersion : EgController
    {
        public CreateNewClickwrapVersion(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgName, ExamplesAPIType.Click);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "ClickEg003";

        protected override void InitializeInternal()
        {
            base.InitializeInternal();

            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = RequestItemsService.Session.AccountId;
            ViewBag.ClickwrapsData = DocuSign.Click.Examples.RetrieveClickwraps.GetClickwraps(basePath, accessToken, accountId);
            ViewBag.AccountId = RequestItemsService.Session.AccountId;
        }

        [MustAuthenticate]
        [SetViewBag]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string clickwrapId, string clickwrapName)
        {
            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = $"{this.RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = this.RequestItemsService.Session.AccountId;

            try
            {
                // Call the Click API to create the clickwrap version
                var clickWrap = DocuSign.Click.Examples.CreateNewClickwrapVersion.Create(clickwrapId, basePath, accessToken, accountId, clickwrapName);

                // Show results
                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, clickWrap.VersionNumber, clickWrap.ClickwrapName);
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
    }
}