// <copyright file="CreateCLMESignUser.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Admin.Controllers
{
    using System;
    using DocuSign.Admin.Api;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Examples;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Admin")]
    [Route("Aeg002")]
    public class CreateCLMESignUser : EgController
    {
        private static Guid? clmProductId;
        private static Guid? eSignProductId;
        public CreateCLMESignUser(
            DSConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgName, ExamplesAPIType.Admin);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Aeg002";

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            var organizationId = this.RequestItemsService.OrganizationId;
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.AdminApiBasePath;
            var accountId = this.RequestItemsService.Session.AccountId;

            // Step 2 Start
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Step 2 End

            // Step 3 Start
            var productPermissionProfileApi = new ProductPermissionProfilesApi(apiClient);
            var productPermissionProfiles = productPermissionProfileApi.GetProductPermissionProfiles(organizationId, Guid.Parse(accountId));
            this.ViewBag.CLMPermissionProfiles = productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "CLM").PermissionProfiles;
            this.ViewBag.ESignPermissionProfiles = productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "ESign").PermissionProfiles;
            clmProductId = productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "CLM").ProductId;
            eSignProductId = productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "ESign").ProductId;

            // Step 3 End

            // Step 4 Start
            var dsGroupsApi = new DSGroupsApi(apiClient);
            this.ViewBag.DsGroups = dsGroupsApi.GetDSGroups(organizationId, Guid.Parse(accountId)).DsGroups;

            // Step 4 End
            if (this.ViewBag.DsGroups.Count == 0)
            {
                throw new ApiException(0, this.CodeExampleText.CustomErrorTexts[0].ErrorMessage);
            }
        }

        [MustAuthenticate]
        [SetViewBag]
        [HttpGet]
        public override IActionResult Get()
        {
            try
            {
                base.Get();
                return this.View("Aeg002", this);
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
        public ActionResult Create(string userName, string firstName, string lastName, string email, string cLMPermissionProfileId, string eSignPermissionProfileId, string dsGroupId)
        {
            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.AdminApiBasePath;
            var accountId = this.RequestItemsService.Session.AccountId;
            var orgId = this.RequestItemsService.OrganizationId;

            try
            {
                var newUser = DocuSign.Admin.Examples.CreateCLMESignUser.Create(userName, firstName, lastName, email, cLMPermissionProfileId, eSignPermissionProfileId, Guid.Parse(dsGroupId), clmProductId, eSignProductId, basePath, accessToken, Guid.Parse(accountId), orgId);

                this.RequestItemsService.EmailAddress = newUser.Email;

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(newUser, Formatting.Indented);
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
