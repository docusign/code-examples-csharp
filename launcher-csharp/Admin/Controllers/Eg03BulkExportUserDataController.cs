﻿// <copyright file="Eg03BulkExportUserDataController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Admin.Controllers
{
    using System;
    using System.IO;
    using DocuSign.Admin.Client;
    using DocuSign.CodeExamples.Admin.Examples;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Admin")]
    [Route("Aeg03")]
    public class Eg03BulkExportUserDataController : EgController
    {
        public Eg03BulkExportUserDataController(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgNumber);
            this.ViewBag.title = this.CodeExampleText.PageTitle;
        }

        public override int EgNumber => 3;

        public override string EgName => "Aeg03";

        [MustAuthenticate]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create()
        {
            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.AdminApiBasePath;
            var organizationId = this.RequestItemsService.OrganizationId;
            var filePath = Path.GetFullPath(Path.Combine(
                Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory),
                this.Config.ExportUsersPath));

            try
            {
                // Call the Admin API to create a new user
                var organizationExportsResponse = BulkExportUserData.CreateBulkExportRequest(
                    accessToken, basePath, organizationId, filePath);

                // Show results
                this.ViewBag.h1 = this.CodeExampleText.ResultsPageHeader;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, filePath);
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(organizationExportsResponse, Formatting.Indented);

                return this.View("example_done");
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;

                return this.View("Error");
            }
        }
    }
}
