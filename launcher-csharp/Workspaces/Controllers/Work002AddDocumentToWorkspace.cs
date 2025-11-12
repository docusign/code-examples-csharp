// <copyright file="Work002AddDocumentToWorkspace.cs" company="Docusign">
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
    [Route("work002")]
    public class Work002AddDocumentToWorkspace : EgController
    {
        public Work002AddDocumentToWorkspace(DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Workspaces);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "work002";

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

            return this.View("work002", this);
        }

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string documentName, string documentPath)
        {
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            var accessToken = this.RequestItemsService.User.AccessToken;
            var accountId = this.RequestItemsService.Session.AccountId;
            var workspaceId = this.RequestItemsService.WorkspaceId;

            var documentResponse = AddDocumentToWorkspaces.AddDocumentToWorkspaceAsync(
                accessToken,
                accountId,
                workspaceId,
                documentName,
                documentPath);

            this.RequestItemsService.WorkspaceDocumentId = documentResponse.Result.DocumentId;

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(
                this.CodeExampleText.ResultsPageText,
                documentResponse.Result.DocumentId);
            this.ViewBag.Locals.Json = JsonConvert.SerializeObject(documentResponse.Result, Formatting.Indented);
            return this.View("example_done");
        }
    }
}
