// <copyright file="GetClickwrapResponses.cs" company="DocuSign">
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
    [Route("ceg005")]
    public class GetClickwrapResponses : EgController
    {
        public GetClickwrapResponses(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Click);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "ceg005";

        [MustAuthenticate]
        [SetViewBag]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string clickwrapId)
        {
            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = $"{this.RequestItemsService.Session.BasePath}/clickapi"; // Base API path

            try
            {
                var accountId = this.RequestItemsService.Session.AccountId;

                // Call the Click API to get a clickwrap agreements
                var clickWrapAgreements = DocuSign.Click.Examples.GetClickwrapResponses.GetAgreements(clickwrapId, basePath, accessToken, accountId);

                // Show results
                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(clickWrapAgreements, Formatting.Indented);

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
            this.ViewBag.ClickwrapsData = DocuSign.Click.Examples.RetrieveClickwraps.GetClickwraps(basePath, accessToken, accountId);
            this.ViewBag.AccountId = this.RequestItemsService.Session.AccountId;
        }
    }
}