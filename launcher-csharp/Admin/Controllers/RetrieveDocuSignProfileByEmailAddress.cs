// <copyright file="RetrieveDocuSignProfileByEmailAddress.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Admin.Controllers
{
    using System;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Model;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Admin")]
    [Route("aeg006")]
    public class RetrieveDocuSignProfileByEmailAddress : EgController
    {
        public RetrieveDocuSignProfileByEmailAddress(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Admin);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "aeg006";

        [MustAuthenticate]
        [SetViewBag]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RetriveProfileByEmail(string email)
        {
            try
            {
                Guid? organizationId = this.RequestItemsService.OrganizationId;
                string accessToken = this.RequestItemsService.User.AccessToken;
                string basePath = this.RequestItemsService.Session.AdminApiBasePath;

                UsersDrilldownResponse profileWithSearchedEmail = DocuSign.Admin.Examples.RetrieveDocuSignProfileByEmailAddress.
                    GetDocuSignProfileByEmailAdress(basePath, accessToken, organizationId, email);

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(profileWithSearchedEmail, Formatting.Indented);

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