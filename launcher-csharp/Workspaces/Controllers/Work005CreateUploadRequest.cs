// <copyright file="Work005CreateUploadRequest.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Examples;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("Workspaces")]
    [Route("work005")]
    public class Work005CreateUploadRequest : EgController
    {
        public Work005CreateUploadRequest(DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Workspaces);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "work005";

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
            this.ViewBag.CreatorIdOk = this.RequestItemsService.CreatorId != null;

            return this.View("work005", this);
        }

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string assigneeEmail)
        {
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            var accessToken = this.RequestItemsService.User.AccessToken;
            var accountId = this.RequestItemsService.Session.AccountId;
            var creatorId = this.RequestItemsService.CreatorId;
            var workspaceId = this.RequestItemsService.WorkspaceId;

            try
            {
                var results = CreateUploadRequest.CreateUploadRequestWithDueDate(
                    accessToken,
                    accountId,
                    workspaceId,
                    creatorId,
                    assigneeEmail);

                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, results.Result.UploadRequestId);
            }
            catch (Exception ex)
            {
                this.ViewBag.message = this.CodeExampleText.CustomErrorTexts[0].ErrorMessage;
            }

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            return this.View("example_done");
        }
    }
}
