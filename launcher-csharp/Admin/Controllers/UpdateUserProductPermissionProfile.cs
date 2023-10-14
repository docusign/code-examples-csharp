// <copyright file="UpdateUserProductPermissionProfile.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using DocuSign.Admin.Api;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Examples;
    using DocuSign.Admin.Model;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Admin")]
    [Route("aeg008")]
    public class UpdateUserProductPermissionProfile : EgController
    {
        private static ProductPermissionProfilesResponse productPermissionProfiles;

        private static Dictionary<Guid?, string> products = new Dictionary<Guid?, string>();

        public UpdateUserProductPermissionProfile(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Admin);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "aeg008";

        [Route("/getPermissionProfiles")]
        public IActionResult GetPermissionProfiles(Guid? productId)
        {
            return this.Json(productPermissionProfiles.ProductPermissionProfiles
                .Find(x => x.ProductId == productId)?.PermissionProfiles);
        }

        [MustAuthenticate]
        [SetViewBag]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUsersProductPermissionProfile(string productId, string permissionProfileId)
        {
            try
            {
                Guid? organizationId = this.RequestItemsService.OrganizationId;
                string accessToken = this.RequestItemsService.User.AccessToken;
                string basePath = this.RequestItemsService.Session.AdminApiBasePath;
                Guid accountId = Guid.Parse(this.RequestItemsService.Session.AccountId);
                string email = this.RequestItemsService.EmailAddress;

                UserProductPermissionProfilesResponse response = UpdateUserProductPermissionProfileByEmail.UpdateUserProductPermissionProfile(
                    basePath,
                    accessToken,
                    organizationId,
                    accountId,
                    email,
                    Guid.Parse(productId),
                    permissionProfileId);

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(response, Formatting.Indented);

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
            Guid? organizationId = this.RequestItemsService.OrganizationId;
            string accessToken = this.RequestItemsService.User.AccessToken;
            string basePath = this.RequestItemsService.Session.AdminApiBasePath;
            Guid accountId = Guid.Parse(this.RequestItemsService.Session.AccountId);

            productPermissionProfiles = UpdateUserProductPermissionProfileByEmail
                .GetPermissionProfiles(basePath, accessToken, organizationId, accountId);

            this.ViewBag.CLMPermissionProfiles = productPermissionProfiles.ProductPermissionProfiles
                .Find(x => x.ProductName == "CLM").PermissionProfiles;

            products.TryAdd(
                productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "CLM").ProductId,
                "CLM");

            products.TryAdd(
                productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "ESign").ProductId,
                "eSignature");

            this.ViewBag.Products = products;
            this.ViewBag.EmailAddress = this.RequestItemsService.EmailAddress;
        }
    }
}