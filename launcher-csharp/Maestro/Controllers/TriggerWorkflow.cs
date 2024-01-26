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
            CodeExampleText = GetExampleText(EgName, ExamplesApiType.Maestro);
            ViewBag.title = CodeExampleText.ExampleName;
        }

        public override string EgName => "mae001";

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            try
            {
                var actionResult = base.Get();
                if (RequestItemsService.EgName == EgName)
                {
                    return actionResult;
                }

                RequestItemsService.WorkflowId = configuration["DocuSign:WorkflowId"];
                var accessToken = RequestItemsService.User.AccessToken;
                var accountId = RequestItemsService.Session.AccountId;

                if (!RequestItemsService.WorkflowPublished)
                {
                    var docuSignClient = new DocuSignClient(RequestItemsService.Session.MaestroManageApiBasePath);
                    docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
                    TriggerWorkflowService.PublishWorkFlow(docuSignClient, accountId, RequestItemsService.WorkflowId);
                    RequestItemsService.WorkflowPublished = true;
                }

                ViewBag.Config = Config;
                var workflowTriggerModel = new WorkflowTriggerModel();

                return View("mae001", workflowTriggerModel);
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;
                ViewBag.SupportingTexts = LauncherTexts.ManifestStructure.SupportingTexts;

                return View("Error");
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
                var accessToken = RequestItemsService.User.AccessToken;
                var accountId = RequestItemsService.Session.AccountId;
                var docuSignManageClient = new DocuSignClient(RequestItemsService.Session.MaestroManageApiBasePath);
                docuSignManageClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
                var workflow = TriggerWorkflowService.GetWorkFlowDefinition(docuSignManageClient, accountId, RequestItemsService.WorkflowId);

                var docuSignAuthClient = new DocuSignClient(RequestItemsService.Session.MaestroAuthApiBasePath);
                docuSignAuthClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
                var result = TriggerWorkflowService.TriggerWorkflow(docuSignAuthClient, accountId, new Uri(workflow.TriggerUrl), model);

                RequestItemsService.InstanceId = result.InstanceId;

                ViewBag.h1 = CodeExampleText.ExampleName;
                ViewBag.message = CodeExampleText.ResultsPageText;
                ViewBag.Locals.Json = JsonConvert.SerializeObject(result, Formatting.Indented);

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;
                ViewBag.SupportingTexts = LauncherTexts.ManifestStructure.SupportingTexts;

                return View("Error");
            }
        }
    }
}
