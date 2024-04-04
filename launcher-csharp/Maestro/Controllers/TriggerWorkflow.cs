// <copyright file="TriggerWorkflow.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Maestro.Controllers
{
    using System;
    using System.Linq;
    using DocuSign.CodeExamples;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Maestro.Models;
    using DocuSign.CodeExamples.Models;
    using DocuSign.Maestro.Client;
    using DocuSign.Maestro.Model;
    using DocuSign.WebForms.Examples;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    [Area("Maestro")]
    [Route("mae001")]
    public class TriggerWorkflow : EgController
    {
        private const string WORKFLOWNAME = "Example workflow - send invite to signer";

        private IConfiguration configuration;

        public TriggerWorkflow(
            DsConfiguration config,
            IConfiguration configuration,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.configuration = configuration;
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Maestro);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
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

            var accessToken = this.RequestItemsService.User.AccessToken;
            var accountId = this.RequestItemsService.Session.AccountId;

            try
            {
                //ds-snippet-start:Maestro1Step2
                var docuSignManageClient = new DocuSignClient(this.RequestItemsService.Session.MaestroApiBasePath);
                docuSignManageClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
                //ds-snippet-end:Maestro1Step2
                var workflows = TriggerWorkflowService.GetWorkFlowDefinitions(docuSignManageClient, accountId);

                if (workflows.Count > 0)
                {
                    var workflow = workflows.Value
                        .Where(x => x.Name == WORKFLOWNAME)
                        .OrderByDescending(x => x.LastUpdatedDate)
                        .FirstOrDefault();
                    this.RequestItemsService.WorkflowId = workflow != null ? workflow.Id : null;
                }

                if (this.RequestItemsService.WorkflowId == null && this.RequestItemsService.TemplateId != null)
                {
                    NewOrUpdatedWorkflowDefinitionResponse workflow = CreateWorkflowService.CreateWorkflowDefinition(
                        docuSignManageClient,
                        accountId,
                        this.RequestItemsService.TemplateId);
                    this.RequestItemsService.WorkflowId = workflow.WorkflowDefinitionId;

                    string publishLink = TriggerWorkflowService.PublishWorkFlow(
                        docuSignManageClient,
                        accountId,
                        workflow.WorkflowDefinitionId);
                    this.RequestItemsService.IsWorkflowPublished = true;
                    this.ViewBag.ConsentLink = this.CodeExampleText.AdditionalPages[0].ResultsPageText
                        .Replace("{0}", publishLink);

                    return this.View("publishWorkflow");
                }

                if (this.RequestItemsService.IsWorkflowPublished)
                {
                    string publishLink = TriggerWorkflowService.PublishWorkFlow(
                        docuSignManageClient,
                        accountId,
                        this.RequestItemsService.WorkflowId);
                    if (publishLink != string.Empty)
                    {
                        this.ViewBag.ConsentLink = this.CodeExampleText.AdditionalPages[0].ResultsPageText
                            .Replace("{0}", publishLink);

                        return this.View("publishWorkflow");
                    }
                    else
                    {
                        this.RequestItemsService.IsWorkflowPublished = false;
                    }
                }

                this.ViewBag.WorkflowId = this.RequestItemsService.WorkflowId;
                this.ViewBag.TemplateId = this.RequestItemsService.TemplateId;
                this.ViewBag.Config = this.Config;
                var workflowTriggerModel = new WorkflowTriggerModel();

                return this.View("mae001", workflowTriggerModel);
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.ErrorCode == 403 ?
                    this.LauncherTexts.ManifestStructure.SupportingTexts.ContactSupportToEnableFeature.Replace("{0}", "Maestro")
                    : apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }
        }

        [MustAuthenticate]
        [SetViewBag]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(WorkflowTriggerModel model)
        {
            try
            {
                var accessToken = this.RequestItemsService.User.AccessToken;
                var accountId = this.RequestItemsService.Session.AccountId;
                var docuSignManageClient = new DocuSignClient(this.RequestItemsService.Session.MaestroApiBasePath);
                docuSignManageClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

                var workflow = TriggerWorkflowService.GetWorkFlowDefinition(
                    docuSignManageClient,
                    accountId,
                    this.RequestItemsService.WorkflowId);

                var result = TriggerWorkflowService.TriggerWorkflow(
                    docuSignManageClient,
                    accountId,
                    new Uri(workflow.TriggerUrl),
                    model);

                this.RequestItemsService.InstanceId = result.InstanceId;

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(result, Formatting.Indented);

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
    }
}
