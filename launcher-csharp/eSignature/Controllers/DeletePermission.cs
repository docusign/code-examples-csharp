﻿// <copyright file="DeletePermission.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System.Linq;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [Area("eSignature")]
    [Route("Eg027")]
    public class DeletePermission : EgController
    {
        public DeletePermission(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgNumber);
            this.ViewBag.title = this.CodeExampleText.PageTitle;
        }

        public override int EgNumber => 27;

        public override string EgName => "Eg027";

        protected override void InitializeInternal()
        {
            base.InitializeInternal();

            // Data for this method
            // permission profiles
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Get all available permissions
            var accountsApi = new AccountsApi(apiClient);
            var permissions = accountsApi.ListPermissions(accountId);
            this.ViewBag.PermissionProfiles =
            permissions.PermissionProfiles.Select(pr => new SelectListItem
            {
                Text = pr.PermissionProfileName,
                Value = pr.PermissionProfileId,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string permissionProfileId)
        {
            // Check the minimal buffer time.
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            // Uri of rest api
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";

            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Step 3. Call the eSignature REST API
                global::ESignature.Examples.DeletePermission.DeletePermissionProfile(permissionProfileId, accessToken, basePath, accountId);
            }
            catch (ApiException ex)
            {
                // Request failed. Display error text
                this.ViewBag.h1 = "The permission profile failed to delete";
                this.ViewBag.message = $"The permission profile failed to delete.<br /> Reason: <br />{ex.ErrorContent}";
                return this.View("example_done");
            }

            this.ViewBag.h1 = this.CodeExampleText.ResultsPageHeader;
            this.ViewBag.message = this.CodeExampleText.ResultsPageText;
            return this.View("example_done");
        }
    }
}