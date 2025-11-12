// <copyright file="Work003SendEnvelopeWithRecipientInfo.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Examples;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Workspaces")]
    [Route("work003")]
    public class Work003SendEnvelopeWithRecipientInfo : EgController
    {
        public Work003SendEnvelopeWithRecipientInfo(DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Workspaces);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "work003";

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            IActionResult actionResult = base.Get();
            if (this.RequestItemsService.EgName == this.EgName)
            {
                return actionResult;
            }

            this.ViewBag.WorkspaceIdOk = this.RequestItemsService.WorkspaceId != null;
            this.ViewBag.DocumentIdOk = this.RequestItemsService.WorkspaceDocumentId != null;

            return this.View("work003", this);
        }

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName)
        {
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accountId = this.RequestItemsService.Session.AccountId;
            var documentId = this.RequestItemsService.WorkspaceDocumentId;
            var workspaceId = this.RequestItemsService.WorkspaceId;

            var workspaceEnvelope = SendEnvelopeWithRecipientInfo.CreateEnvelopeAsync(
                accessToken,
                accountId,
                documentId,
                workspaceId);
            var envelopeId = workspaceEnvelope.Result.EnvelopeId;

            var results = SendEnvelopeWithRecipientInfo.SendEnvelopeAsync(
                basePath,
                accessToken,
                accountId,
                envelopeId,
                signerEmail,
                signerName);

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, envelopeId);
            return this.View("example_done");
        }
    }
}
