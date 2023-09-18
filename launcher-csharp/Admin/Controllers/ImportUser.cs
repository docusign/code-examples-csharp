// <copyright file="ImportUser.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Admin.Controllers
{
    using System;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Model;
    using DocuSign.CodeExamples.Admin.Examples;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Admin")]
    [Route("aeg004")]
    public class ImportUser : EgController
    {
        public ImportUser(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Admin);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "aeg004";

        [MustAuthenticate]
        [SetViewBag]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create()
        {
            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.AdminApiBasePath;
            var accountId = this.RequestItemsService.Session.AccountId;
            var organizationId = this.RequestItemsService.OrganizationId;

            try
            {
                // Call the Admin API to create a new user
                OrganizationImportResponse organizationImportResponse = DocuSign.CodeExamples.Admin.Examples.ImportUser.CreateBulkImportRequest(
                    accessToken, basePath, accountId, organizationId, this.Config.DocCsv);

                // Show results
                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(organizationImportResponse, Formatting.Indented);
                this.ViewBag.AdditionalLinkText = this.CodeExampleText.AdditionalPages[0].Name;
                this.ViewBag.AdditionalLink = "CheckStatus?id=" + organizationImportResponse.Id;

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

        [MustAuthenticate]
        [SetViewBag]
        [HttpGet]
        [Route("CheckStatus")]
        public ActionResult CheckStatus(string id)
        {
            try
            {
                // Obtain your OAuth token
                var accessToken = this.RequestItemsService.User.AccessToken;
                var basePath = this.RequestItemsService.Session.AdminApiBasePath;
                var organizationId = this.RequestItemsService.OrganizationId;
                OrganizationImportResponse organizationImportResponse = DocuSign.CodeExamples.Admin.Examples.ImportUser.CheckkStatus(accessToken, basePath, organizationId, Guid.Parse(id));

                // Show results
                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.AdditionalPages[0].ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(organizationImportResponse, Formatting.Indented);
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
