// <copyright file="RetrieveClickwraps.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Click.Controllers
{
    using System.Text;
    using DocuSign.Click.Client;
    using DocuSign.Click.Examples;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Click")]
    [Route("ceg004")]
    public class RetrieveClickwraps : EgController
    {
        public RetrieveClickwraps(
            DsConfiguration config,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Click);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "ceg004";

        [MustAuthenticate]
        [SetViewBag]
        [Route("Retrieve")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Retrieve()
        {
            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = $"{this.RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = this.RequestItemsService.Session.AccountId;

            try
            {
                // Call the Click API to get the clickwraps
                var clickwraps = DocuSign.Click.Examples.RetrieveClickwraps.GetClickwraps(basePath, accessToken, accountId);

                var messageBuilder = new StringBuilder();
                clickwraps.Clickwraps.ForEach(cw => messageBuilder.AppendLine($"Clickwrap ID:{cw.ClickwrapId}, Clickwrap Version: {cw.VersionNumber}"));

                // Show results
                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(clickwraps.Clickwraps, Formatting.Indented);

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
            this.ViewBag.ClickwrapId = this.RequestItemsService.ClickwrapId;
            this.ViewBag.AccountId = this.RequestItemsService.Session.AccountId;
        }
    }
}