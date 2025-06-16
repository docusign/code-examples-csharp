// <copyright file="Mae001TriggerWorkflowController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
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
        public Mae001TriggerWorkflowController(DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Maestro);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "mae001";

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

            try
            {
                WorkflowsListSuccess workflowsList = await TriggerMaestroWorkflow.GetMaestroWorkflow(
                    basePath,
                    accessToken,
                    accountId);

                if (workflowsList.Data != null || workflowsList.Data.Workflows.Count > 0)
                {
                    var maestroWorkflow = workflowsList.Data.Workflows.FirstOrDefault(workflow =>
                        workflow.Status == "active" && workflow.Name == "Example workflow - send invite to signers");

                    if (maestroWorkflow != null)
                    {
                        string instanceUrl = await TriggerMaestroWorkflow.TriggerWorkflowInstance(
                            basePath,
                            accessToken,
                            accountId,
                            maestroWorkflow.Id,
                            signerEmail,
                            signerName,
                            ccEmail,
                            ccName,
                            instanceName);

                        this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                        this.ViewBag.Url = instanceUrl;
                        this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText);

                        return this.View("embed");
                    }
                }

                this.ViewBag.errorCode = string.Empty;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
                this.ViewBag.SupportMessage = this.CodeExampleText.CustomErrorTexts[0].ErrorMessage;

                return this.View("Error");
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
