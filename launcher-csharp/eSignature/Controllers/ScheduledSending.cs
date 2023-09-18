// <copyright file="ScheduledSending.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("Eg035")]
    public class ScheduledSending : EgController
    {
        public ScheduledSending(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg035";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName, DateTime resumeDate)
        {
            // Check the token with minimal buffer time.
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation
                // so it could be restarted automatically.
                // But since it should be rare to have a token issue here,
                // we'll make the user re-enter the form data after
                // authentication.
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Call the Examples API method to create and schedule the envelope
            var envelopeId = global::ESignature.Examples.ScheduledSending.ScheduleEnvelope(signerEmail, signerName, accessToken, basePath, accountId, this.Config.DocPdf, resumeDate);

            this.RequestItemsService.EnvelopeId = envelopeId;

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, envelopeId);
            return this.View("example_done");
        }
    }
}