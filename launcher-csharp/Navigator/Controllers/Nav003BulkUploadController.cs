// <copyright file="Nav003BulkUploadController.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Examples;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [Area("Navigator")]
    [Route("nav003")]
    public class Nav003BulkUploadController : EgController
    {
        public Nav003BulkUploadController(DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Navigator);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "nav003";

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            IActionResult actionResult = base.Get();
            if (this.RequestItemsService.EgName == this.EgName)
            {
                return actionResult;
            }

            // Obtain your OAuth token
            string accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = this.RequestItemsService.Session.IamBasePath; // Base API path
            string accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            var agreements = NavigatorMethods.ListAgreementsWithIamClient(basePath, accessToken, accountId).Result;

            this.ViewBag.Agreements = agreements;

            return this.View("nav003", this);
        }

        [MustAuthenticate]
        [HttpPost]
        [SetViewBag]
        [Route("create")]
        public async Task<IActionResult> Create()
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

            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.IamBasePath;
            var accountId = this.RequestItemsService.Session.AccountId;

            var jobName = "Example bulk upload job";
            var bulkJob = await BulkUpload.CreateBulkUpload(basePath, accessToken, accountId, jobName);

            // Store the upload URLs and job ID in TempData to be used in the next step of the example.
            var uploadUrls = bulkJob.Embedded.Documents.Select(doc => doc.Actions.UploadDocument).ToArray();
            this.TempData["UploadUrls"] = uploadUrls;
            this.TempData["JobId"] = bulkJob.Id;

            this.ViewBag.CodeExampleText = this.CodeExampleText;
            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText);

            return this.View("nav003_upload");
        }

        [MustAuthenticate]
        [HttpPost]
        [SetViewBag]
        [Route("upload")]
        public async Task<IActionResult> Upload()
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

            var uploadUrls = this.TempData["UploadUrls"] as string[];
            if (uploadUrls == null || uploadUrls.Length == 0)
            {
                return this.Redirect("/nav003");
            }

            var filePaths = new[]
            {
                this.Config.DocDocx,
                this.Config.DocPdf,
                this.Config.DocHtml,
                this.Config.DocTxt,
                this.Config.DocJpg,
            };

            await BulkUpload.UploadDocuments(uploadUrls, filePaths);

            this.ViewBag.CodeExampleText = this.CodeExampleText;
            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.AdditionalPages.FirstOrDefault(page => page.Name == "upload_documents")?.ResultsPageText);

            return this.View("nav003_complete");
        }

        [MustAuthenticate]
        [HttpPost]
        [SetViewBag]
        [Route("complete")]
        public async Task<IActionResult> Complete()
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

            var jobId = this.TempData["JobId"].ToString();
            if (string.IsNullOrEmpty(jobId))
            {
                return this.Redirect("/nav003");
            }

            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.IamBasePath;
            var accountId = this.RequestItemsService.Session.AccountId;

            var bulkJob = await BulkUpload.CompleteBulkUploadJob(basePath, accessToken, accountId, jobId);

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.AdditionalPages.FirstOrDefault(page => page.Name == "job_completed")?.ResultsPageText);
            this.SetLocalsJson(JsonConvert.SerializeObject(bulkJob, Formatting.Indented));

            return this.View("example_done");
        }

        private void SetLocalsJson(string json)
        {
            var locals = this.ViewBag.Locals as Locals ?? new Locals();
            locals.Json = json;
            this.ViewBag.Locals = locals;
        }

        private string[] GetUploadUrls(object bulkJob)
        {
            var documents = JObject.FromObject(bulkJob)["embedded"]?["documents"] as JArray;
            if (documents == null)
            {
                return Array.Empty<string>();
            }

            return documents
                .Select(doc => doc?["actions"]?["upload_document"]?.ToString())
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .ToArray();
        }
    }
}
