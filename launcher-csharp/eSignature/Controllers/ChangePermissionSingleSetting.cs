// <copyright file="ChangePermissionSingleSetting.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("Eg026")]
    public class ChangePermissionSingleSetting : EgController
    {
        public ChangePermissionSingleSetting(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg026";

        [BindProperty]
        public List<PermissionProfile> PermissionProfiles { get; set; }

        [BindProperty]
        public PermissionProfileModel ProfileModel { get; set; }

        [HttpPost]
        [SetViewBag]
        [Route("Create")]
        public IActionResult Create(PermissionProfileModel profileModel)
        {
            // Check the token with minimal buffer time.
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            // Data for this method
            // signerEmail
            // signerName
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";

            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the eSignature REST API
                var results = global::ESignature.Examples.ChangePermissionSingleSetting.UpdatePermissionProfile(
                    profileModel.ProfileId, accessToken, basePath, accountId);

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, results.PermissionProfileId);
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

        protected override void InitializeInternal()
        {
            base.InitializeInternal();

            // Data for this method
            // signerEmail
            // signerName
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var accountsApi = new AccountsApi(docuSignClient);
            var permissions = accountsApi.ListPermissions(accountId);
            this.PermissionProfiles = permissions.PermissionProfiles;
            var permissionProfile = permissions.PermissionProfiles.FirstOrDefault();
            this.ProfileModel = new PermissionProfileModel
            {
                ProfileId = permissionProfile.PermissionProfileId,
                ProfileName = permissionProfile.PermissionProfileName,
                AccountRoleSettingsModel = new AccountRoleSettingsModel(permissionProfile.Settings),
            };
        }
    }
}