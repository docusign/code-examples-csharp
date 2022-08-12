// <copyright file="Eg01CreateUserController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Admin.Controllers
{
    using System;
    using DocuSign.Admin.Client;
    using DocuSign.CodeExamples.Admin.Examples;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Admin")]
    [Route("Aeg01")]
    public class Eg01CreateUserController : EgController
    {
        public override int EgNumber => 1;

        public Eg01CreateUserController(
            DSConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgNumber);
            this.ViewBag.title = this.CodeExampleText.PageTitle;
        }

        public override string EgName => "Aeg01";

        protected override void InitializeInternal()
        {
            base.InitializeInternal();

            try
            {
                var accessToken = this.RequestItemsService.User.AccessToken;
                var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
                var accountId = this.RequestItemsService.Session.AccountId;

                var (permissionProfiles, groups) = CreateUser.GetPermissionProfilesAndGroups(accessToken, basePath, accountId);

                this.ViewBag.PermissionProfiles = permissionProfiles.PermissionProfiles;
                this.ViewBag.Groups = groups.Groups;
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
            }
        }

        [MustAuthenticate]
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
                var user = CreateUser.CreateNewUser(
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
                this.ViewBag.h1 = this.CodeExampleText.ResultsPageHeader;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(user, Formatting.Indented);

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
