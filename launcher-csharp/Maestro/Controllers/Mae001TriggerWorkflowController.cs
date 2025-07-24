// <copyright file="Mae001TriggerWorkflowController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Examples;
    using DocuSign.CodeExamples.Models;
    using Docusign.IAM.SDK.Models.Components;
    using Docusign.IAM.SDK.Models.Errors;
    using Microsoft.AspNetCore.Mvc;

    [Area("Maestro")]
    [Route("mae001")]
    public class Mae001TriggerWorkflowController : EgController
    {
        private const string WorkflowName = "Example workflow - send invite to signer";

        public Mae001TriggerWorkflowController(DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Maestro);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "mae001";

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            var actionResult = base.Get();
            if (this.RequestItemsService.EgName == this.EgName)
            {
                return actionResult;
            }

            var basePath = this.RequestItemsService.Session.IamBasePath;
            var accessToken = this.RequestItemsService.User.AccessToken;
            var accountId = this.RequestItemsService.Session.AccountId;

            try
            {
                if (this.RequestItemsService.WorkflowId == null)
                {
                    WorkflowsListSuccess workflowsList = TriggerMaestroWorkflow.GetMaestroWorkflow(
                        basePath,
                        accessToken,
                        accountId).GetAwaiter().GetResult();

                    if (workflowsList.Data != null || workflowsList.Data.Count > 0)
                    {
                        var maestroWorkflow = workflowsList.Data.FirstOrDefault(workflow => workflow.Name == WorkflowName);

                        this.RequestItemsService.WorkflowId = maestroWorkflow?.Id;
                    }
                }

                if (this.RequestItemsService.WorkflowId == null && this.RequestItemsService.TemplateId != null)
                {
                    var workflowId = TriggerMaestroWorkflow.CreateWorkflowAsync(
                        accessToken,
                        accountId,
                        this.RequestItemsService.TemplateId,
                        this.Config.MaestroWorkflowConfig).GetAwaiter().GetResult();
                    this.RequestItemsService.WorkflowId = workflowId;

                    var publishLink = TriggerMaestroWorkflow.PublishWorkflowAsync(
                        accountId,
                        workflowId,
                        accessToken).GetAwaiter().GetResult();

                    this.RequestItemsService.IsWorkflowPublished = true;
                    this.ViewBag.ConsentLink = this.CodeExampleText.AdditionalPages[0].ResultsPageText
                        .Replace("{0}", publishLink);
                    this.ViewBag.WorkflowId = this.RequestItemsService.WorkflowId;
                    this.ViewBag.TemplateId = this.RequestItemsService.TemplateId;

                    return this.View("publishWorkflow");
                }

                if (this.RequestItemsService.IsWorkflowPublished)
                {
                    var publishLink = TriggerMaestroWorkflow.PublishWorkflowAsync(
                        accountId,
                        this.RequestItemsService.WorkflowId,
                        accessToken).GetAwaiter().GetResult();

                    if (publishLink != string.Empty)
                    {
                        this.ViewBag.ConsentLink = this.CodeExampleText.AdditionalPages[0].ResultsPageText
                            .Replace("{0}", publishLink);

                        this.ViewBag.WorkflowId = this.RequestItemsService.WorkflowId;
                        this.ViewBag.TemplateId = this.RequestItemsService.TemplateId;
                        return this.View("publishWorkflow");
                    }

                    this.RequestItemsService.IsWorkflowPublished = false;
                }

                this.ViewBag.WorkflowId = this.RequestItemsService.WorkflowId;
                this.ViewBag.TemplateId = this.RequestItemsService.TemplateId;

                return this.View("mae001");
            }
            catch (Exception apiException)
            {
                this.ViewBag.errorCode = string.Empty;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
                this.ViewBag.SupportMessage = this.CodeExampleText.CustomErrorTexts[0].ErrorMessage;

                return this.View("Error");
            }
        }

        [HttpPost]
        [SetViewBag]
        public async Task<IActionResult> Create(
            string instanceName,
            string signerEmail,
            string signerName,
            string ccEmail,
            string ccName)
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
                var instance = await TriggerMaestroWorkflow.TriggerWorkflowInstance(
                    basePath,
                    accessToken,
                    accountId,
                    workflowId,
                    signerEmail,
                    signerName,
                    ccEmail,
                    ccName,
                    instanceName);

                this.RequestItemsService.WorkflowId = workflowId;
                this.RequestItemsService.InstanceId = instance.InstanceId;

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.Url = instance.InstanceUrl;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText);

                return this.View("embed");
            }
            catch (APIException exception)
            {
                this.ViewBag.errorCode = exception.StatusCode;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
                this.ViewBag.errorMessage = exception.Message;

                return this.View("Error");
            }
        }
    }
}
