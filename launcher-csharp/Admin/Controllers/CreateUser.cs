// <copyright file="CreateUser.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Admin.Controllers
{
    using System;
    using DocuSign.Admin.Client;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Admin")]
    [Route("aeg001")]
    public class CreateUser : EgController
    {
        public CreateUser(
            DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Admin);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "aeg001";

        [MustAuthenticate]
        [SetViewBag]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string email, string firstName, string lastName, string userName, string permissionProfileId, string groupId)
        {
            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.AdminApiBasePath;
            var accountId = this.RequestItemsService.Session.AccountId;
            var organizationId = this.RequestItemsService.OrganizationId;

            try
            {
                // Call the Admin API to create a new user
                var user = DocuSign.CodeExamples.Admin.Examples.CreateUser.CreateNewUser(
                    accessToken,
                    basePath,
                    Guid.Parse(accountId),
                    organizationId,
                    firstName,
                    lastName,
                    userName,
                    email,
                    long.Parse(permissionProfileId),
                    long.Parse(groupId));

                // Show results
                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(user, Formatting.Indented);

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

            try
            {
                var accessToken = this.RequestItemsService.User.AccessToken;
                var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
                var accountId = this.RequestItemsService.Session.AccountId;

                var (permissionProfiles, groups) = Examples.CreateUser.GetPermissionProfilesAndGroups(accessToken, basePath, accountId);

                this.ViewBag.PermissionProfiles = permissionProfiles.PermissionProfiles;
                this.ViewBag.Groups = groups.Groups;
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
            }
        }
    }
}
