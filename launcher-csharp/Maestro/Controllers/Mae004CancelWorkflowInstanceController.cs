// <copyright file="Mae004CancelWorkflowInstanceController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
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
    [Route("mae004")]
    public class Mae004CancelWorkflowInstanceController : EgController
    {
        private const int StatusCode = 500;

        public Mae004CancelWorkflowInstanceController(DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Maestro);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "mae004";

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            IActionResult actionResult = base.Get();
            if (this.RequestItemsService.EgName == this.EgName)
            {
                return actionResult;
            }

            this.ViewBag.IsWorkflowIdPresent = this.RequestItemsService.WorkflowId != null &&
                                               this.RequestItemsService.InstanceId != null;

            return this.View("mae004", this);
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
            string workflowInstanceId = this.RequestItemsService.InstanceId;

            try
            {
                var cancelWorkflowInstanceResponse = CancelWorkflowInstance.CancelInstanceMaestroWorkflow(
                    basePath,
                    accessToken,
                    accountId,
                    workflowId,
                    workflowInstanceId).Result;

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText);
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(cancelWorkflowInstanceResponse, Formatting.Indented);

                return this.View("example_done");
            }
            catch (Exception exception)
            {
                this.ViewBag.errorCode = StatusCode;
                this.ViewBag.fixingInstructions = string.Format(
                    this.CodeExampleText.CustomErrorTexts[1].ErrorMessage,
                    this.RequestItemsService.InstanceId);
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
                this.ViewBag.errorMessage = exception.Message;

                return this.View("Error");
            }
        }
    }
}
