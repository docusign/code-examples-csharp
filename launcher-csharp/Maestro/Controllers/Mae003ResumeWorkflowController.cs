// <copyright file="Mae003ResumeWorkflowController.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Examples;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Maestro")]
    [Route("mae003")]
    public class Mae003ResumeWorkflowController : EgController
    {
        private const int StatusCode = 500;

        public Mae003ResumeWorkflowController(DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Maestro);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "mae003";

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            IActionResult actionResult = base.Get();
            if (this.RequestItemsService.EgName == this.EgName)
            {
                return actionResult;
            }

            this.ViewBag.IsWorkflowIdPresent = this.RequestItemsService.WorkflowId != null;

            return this.View("mae003", this);
        }

        [HttpPost]
        [SetViewBag]
        public IActionResult Create()
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

            string accessToken = this.RequestItemsService.User.AccessToken;
            string basePath = this.RequestItemsService.Session.IamBasePath;
            string accountId = this.RequestItemsService.Session.AccountId;
            string workflowId = this.RequestItemsService.WorkflowId;

            try
            {
                var resumeWorkflowResponse = ResumeWorkflow.ResumeMaestroWorkflow(
                    basePath,
                    accessToken,
                    accountId,
                    workflowId).Result;

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText);
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(resumeWorkflowResponse, Formatting.Indented);

                return this.View("example_done");
            }
            catch (Exception exception)
            {
                this.ViewBag.fixingInstructions = string.Format(
                    this.CodeExampleText.CustomErrorTexts[1].ErrorMessage,
                    this.RequestItemsService.WorkflowId);
                this.ViewBag.errorCode = StatusCode;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
                this.ViewBag.errorMessage = exception.Message;

                return this.View("Error");
            }
        }
    }
}
