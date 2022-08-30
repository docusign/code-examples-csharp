// <copyright file="Eg07RetrieveDocuSignProfileByUserId.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Admin.Controllers
{
    using System;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Examples;
    using DocuSign.Admin.Model;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Admin")]
    [Route("Aeg07")]
    public class RetrieveDocuSignProfileByUserId : EgController
    {
        public override int EgNumber => 7;

        public RetrieveDocuSignProfileByUserId(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgNumber);
            this.ViewBag.title = this.CodeExampleText.PageTitle;
        }

        public override string EgName => "Aeg07";

        [MustAuthenticate]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RetriveProfileById(Guid userId)
        {
            try
            {
                Guid? organizationId = this.RequestItemsService.OrganizationId;
                string accessToken = this.RequestItemsService.User.AccessToken;
                string basePath = this.RequestItemsService.Session.AdminApiBasePath;

                UsersDrilldownResponse usersData = DocuSign.Admin.Examples.RetrieveDocuSignProfileByUserId.
                    GetDocuSignProfileByUserId(basePath, accessToken, organizationId, userId);

                this.ViewBag.h1 = this.CodeExampleText.ResultsPageHeader;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(usersData, Formatting.Indented);

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