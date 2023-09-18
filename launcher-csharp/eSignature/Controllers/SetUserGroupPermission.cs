// <copyright file="SetUserGroupPermission.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System.Linq;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [Area("eSignature")]
    [Route("Eg025")]
    public class SetUserGroupPermission : EgController
    {
        public SetUserGroupPermission(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg025";

        [HttpPost]
        [SetViewBag]
        [ValidateAntiForgeryToken]
        public IActionResult SetProfile(string permissionProfileId, string userGroupId)
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

            // Call the Examples API method to set permissions to the specified user group
            var result = global::ESignature.Examples.SetUserGroupPermission.GetGroupInformation(
                permissionProfileId,
                userGroupId,
                accessToken,
                basePath,
                accountId);

            var errorDetails = result.Groups.FirstOrDefault()?.ErrorDetails;

            if (errorDetails is null)
            {
                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
            }
            else
            {
                this.ViewBag.h1 = this.CodeExampleText.CustomErrorTexts[0].ErrorMessage;
                this.ViewBag.message = this.CodeExampleText.CustomErrorTexts[0].ErrorMessage + "< br /> " +
                                  $"Reason: {errorDetails.Message}";
            }

            return this.View("example_done");
        }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();

            // Data for this method
            // Permission profiles
            // User groups
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var accountsApi = new AccountsApi(docuSignClient);
            var groupsApi = new GroupsApi(docuSignClient);
            var permissions = accountsApi.ListPermissions(accountId);
            var userGroups = groupsApi.ListGroups(accountId);

            // List all available permission profiles
            this.ViewBag.PermissionProfiles =
                permissions.PermissionProfiles.Select(pr => new SelectListItem
                {
                    Text = pr.PermissionProfileName,
                    Value = pr.PermissionProfileId,
                });

            // List all available user groups
            this.ViewBag.UserGroups =
                userGroups.Groups.Select(pr => new SelectListItem
                {
                    Text = $"{pr.GroupName}",
                    Value = pr.GroupId,
                });
        }
    }
}