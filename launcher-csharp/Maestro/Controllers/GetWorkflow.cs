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
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Maestro);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "mae003";

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

                this.ViewBag.WorkflowId = this.RequestItemsService.WorkflowId;
                this.ViewBag.InstanceId = this.RequestItemsService.InstanceId;

                return this.View("mae003");
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
        public ActionResult GetFlow()
        {
            try
            {
                var accessToken = this.RequestItemsService.User.AccessToken;
                var accountId = this.RequestItemsService.Session.AccountId;
                var docuSignClient = new DocuSignClient(this.RequestItemsService.Session.MaestroApiBasePath);
                docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
                var result = GetWorkflowService.GetWorkFlowInstance(
                    docuSignClient,
                    accountId,
                    this.RequestItemsService.WorkflowId,
                    this.RequestItemsService.InstanceId);

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, result.InstanceState);
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
