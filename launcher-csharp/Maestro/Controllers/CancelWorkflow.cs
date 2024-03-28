// <copyright file="CancelWorkflow.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Maestro.Controllers
{
    using DocuSign.CodeExamples;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.Maestro.Client;
    using DocuSign.Maestro.Model;
    using DocuSign.WebForms.Examples;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    [Area("Maestro")]
    [Route("mae002")]
    public class CancelWorkflow : EgController
    {
        private IConfiguration configuration;

        public CancelWorkflow(
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

        public override string EgName => "mae002";

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

                var accessToken = this.RequestItemsService.User.AccessToken;
                var accountId = this.RequestItemsService.Session.AccountId;
                var docuSignClient = new DocuSignClient(this.RequestItemsService.Session.MaestroApiBasePath);
                docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
                bool isInProgressStatus = false;
                if (this.RequestItemsService.InstanceId != null)
                {
                    var instance = GetWorkflowService.GetWorkFlowInstance(
                        docuSignClient,
                        accountId,
                        this.RequestItemsService.WorkflowId,
                        this.RequestItemsService.InstanceId);
                    isInProgressStatus = instance.InstanceState == WorkflowInstanceState.InProgress;
                }

                this.ViewBag.WorkflowId = this.RequestItemsService.WorkflowId;
                this.ViewBag.InstanceId = this.RequestItemsService.InstanceId;
                this.ViewBag.IsInProgressStatus = isInProgressStatus;

                return this.View("mae002");
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
        public ActionResult CancelFlow()
        {
            try
            {
                var accessToken = this.RequestItemsService.User.AccessToken;
                var accountId = this.RequestItemsService.Session.AccountId;
                var docuSignClient = new DocuSignClient(this.RequestItemsService.Session.MaestroApiBasePath);
                docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
                var result = CancelWorkflowService.CancelWorkflow(docuSignClient, accountId, this.RequestItemsService.InstanceId);

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, this.RequestItemsService.InstanceId);
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
