// <copyright file="Work001CreateWorkspace.cs" company="Docusign">
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
    [Route("work001")]
    public class Work001CreateWorkspace : EgController
    {
        public Work001CreateWorkspace(DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Workspaces);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "work001";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string workspaceName)
        {
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            var accessToken = this.RequestItemsService.User.AccessToken;
            var accountId = this.RequestItemsService.Session.AccountId;

            var createWorkspaceResponse = CreateWorkspace.CreateWorkspaceAsync(
                accessToken,
                accountId,
                workspaceName);

            this.RequestItemsService.WorkspaceId = createWorkspaceResponse.Result.WorkspaceId;
            this.RequestItemsService.CreatorId = createWorkspaceResponse.Result.CreatedByUserId;

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(
                this.CodeExampleText.ResultsPageText,
                createWorkspaceResponse.Result.WorkspaceId);
            this.ViewBag.Locals.Json = JsonConvert.SerializeObject(createWorkspaceResponse.Result, Formatting.Indented);
            return this.View("example_done");
        }
    }
}
