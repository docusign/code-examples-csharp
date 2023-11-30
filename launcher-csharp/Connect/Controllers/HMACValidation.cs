// <copyright file="ValidateWebhookMessage.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Connect.Controllers
{
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.Connect.Examples;
    using Microsoft.AspNetCore.Mvc;

    [Area("Connect")]
    [Route("con001")]
    public class ValidateWebhookMessage : EgController
    {
        public ValidateWebhookMessage(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Connect);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "con001";

        [SetViewBag]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ValidateMessageUsing(string hmacSecret, string jsonPayload)
        {
            jsonPayload = jsonPayload.Replace("\\", string.Empty);

            string hashedOutput = HMACValidation.ComputeHash(hmacSecret, jsonPayload);

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = this.CodeExampleText.ResultsPageText.Replace("{0}", hashedOutput);

            return this.View("example_done");
        }
    }
}