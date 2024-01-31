// <copyright file="TriggerWorkflow.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Maestro.Controllers
{
    using System;
    using DocuSign.CodeExamples;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Maestro.Models;
    using DocuSign.CodeExamples.Models;
    using DocuSign.Maestro.Client;
    using DocuSign.WebForms.Examples;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    [Area("Maestro")]
    [Route("mae001")]
    public class TriggerWorkflow : EgController
    {
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
        }

        public override string EgName => "mae001";

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            try
            {
                var actionResult = base.Get();
                if (this.RequestItemsService.EgName == this.EgName)
                {
                    return actionResult;
                }

                this.RequestItemsService.WorkflowId = this.configuration["DocuSign:WorkflowId"];
                this.ViewBag.Config = this.Config;
                var workflowTriggerModel = new WorkflowTriggerModel();

                return this.View("mae001", workflowTriggerModel);
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
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
                var docuSignManageClient = new DocuSignClient(this.RequestItemsService.Session.MaestroManageApiBasePath);
                docuSignManageClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
                var workflow = TriggerWorkflowService.GetWorkFlowDefinition(docuSignManageClient, accountId, this.RequestItemsService.WorkflowId);

                var docuSignAuthClient = new DocuSignClient(this.RequestItemsService.Session.MaestroAuthApiBasePath);
                docuSignAuthClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
                var result = TriggerWorkflowService.TriggerWorkflow(docuSignAuthClient, accountId, new Uri(workflow.TriggerUrl), model);

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
