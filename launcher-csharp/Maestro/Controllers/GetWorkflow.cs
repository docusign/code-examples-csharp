// <copyright file="GetWorkflow.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Maestro.Controllers
{
    using DocuSign.CodeExamples;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.Maestro.Client;
    using DocuSign.WebForms.Examples;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    [Area("Maestro")]
    [Route("mae003")]
    public class GetWorkflow : EgController
    {
        public const string TemplateName = "Web Form Example Template";

        private IConfiguration configuration;

        public GetWorkflow(
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

        public override string EgName => "mae003";

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

                ViewBag.WorkflowId = RequestItemsService.WorkflowId;
                ViewBag.InstanceId = RequestItemsService.InstanceId;

                return View("mae003");
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
        public ActionResult GetFlow()
        {
            try
            {
                var accessToken = RequestItemsService.User.AccessToken;
                var accountId = RequestItemsService.Session.AccountId;
                var docuSignClient = new DocuSignClient(RequestItemsService.Session.MaestroManageApiBasePath);
                docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
                var result = GetWorkflowService.GetWorkFlowInstance(docuSignClient, accountId, RequestItemsService.WorkflowId, RequestItemsService.InstanceId);

                ViewBag.h1 = CodeExampleText.ExampleName;
                ViewBag.message = string.Format(CodeExampleText.ResultsPageText, result.InstanceState);
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
