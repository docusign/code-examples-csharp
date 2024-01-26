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
            CodeExampleText = GetExampleText(EgName, ExamplesApiType.Maestro);
            ViewBag.title = CodeExampleText.ExampleName;
        }

        public override string EgName => "mae002";

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

                var accessToken = RequestItemsService.User.AccessToken;
                var accountId = RequestItemsService.Session.AccountId;
                var docuSignClient = new DocuSignClient(RequestItemsService.Session.MaestroManageApiBasePath);
                docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
                bool isInProgressStatus = false;
                if(RequestItemsService.InstanceId != null)
                {
                    var instance = GetWorkflowService.GetWorkFlowInstance(docuSignClient, accountId, RequestItemsService.WorkflowId, RequestItemsService.InstanceId);
                    isInProgressStatus = instance.InstanceState == WorkflowInstance.WorkflowInstanceState.InProgress;
                }

                ViewBag.WorkflowId = RequestItemsService.WorkflowId;
                ViewBag.InstanceId = RequestItemsService.InstanceId;
                ViewBag.IsInProgressStatus = isInProgressStatus;

                return View("mae002");
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
        public ActionResult CancelFlow()
        {
            try
            {
                var accessToken = RequestItemsService.User.AccessToken;
                var accountId = RequestItemsService.Session.AccountId;
                var docuSignClient = new DocuSignClient(RequestItemsService.Session.MaestroManageApiBasePath);
                docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
                var result = CancelWorkflowService.CancelWorkflow(docuSignClient, accountId, RequestItemsService.InstanceId);

                ViewBag.h1 = CodeExampleText.ExampleName;
                ViewBag.message = string.Format(CodeExampleText.ResultsPageText, RequestItemsService.InstanceId);
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
