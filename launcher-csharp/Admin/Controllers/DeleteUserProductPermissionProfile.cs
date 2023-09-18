// <copyright file="DeleteUserProductPermissionProfile.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Examples;
    using DocuSign.Admin.Model;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Admin")]
    [Route("aeg009")]
    public class DeleteUserProductPermissionProfile : EgController
    {
        public const string CLMPROFILESNOTFOUND = "No CLM permission profiles are connected to this user";

        public const string ESIGNPROFILESNOTFOUND = "No eSignature permission profiles are connected to this user";

        private static Dictionary<Guid?, string> products;

        private static Guid clmProductId = Guid.Parse("37f013eb-7012-4588-8028-357b39fdbd00");

        private static Guid eSignatureProductId = Guid.Parse("f6406c68-225c-4e9b-9894-64152a26fa83");

        public DeleteUserProductPermissionProfile(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Admin);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "aeg009";

        [MustAuthenticate]
        [SetViewBag]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserProductProfile(string productId)
        {
            try
            {
                Guid? organizationId = this.RequestItemsService.OrganizationId;
                string accessToken = this.RequestItemsService.User.AccessToken;
                string basePath = this.RequestItemsService.Session.AdminApiBasePath;
                Guid accountId = Guid.Parse(this.RequestItemsService.Session.AccountId);
                string email = this.RequestItemsService.EmailAddress;

                RemoveUserProductsResponse response = DeleteUserProductPermissionProfileById.DeleteUserProductPermissionProfile(
                    basePath,
                    accessToken,
                    organizationId,
                    accountId,
                    email,
                    Guid.Parse(productId));

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
            products = new Dictionary<Guid?, string>();

            if (this.RequestItemsService.EmailAddress != null)
            {
                var profiles = DeleteUserProductPermissionProfileById.GetPermissionProfilesByEmail(
                    basePath,
                    accessToken,
                    organizationId,
                    accountId,
                    this.RequestItemsService.EmailAddress);

                var clmProfiles = profiles.ProductPermissionProfiles
                    .Find(x => x.ProductName == "CLM")?
                    .PermissionProfiles
                        .Select(x => x.PermissionProfileName).ToList();

                var eSignProfiles = profiles.ProductPermissionProfiles
                    .Find(x => x.ProductName == "ESign")?
                    .PermissionProfiles?
                        .Select(x => x.PermissionProfileName).ToList();

                var clmProfilesFormatted = clmProfiles != null
                    ? string.Join(",", clmProfiles)
                    : CLMPROFILESNOTFOUND;

                var eSignProfilesFormatted = eSignProfiles != null
                    ? string.Join(",", eSignProfiles)
                    : ESIGNPROFILESNOTFOUND;

                products.TryAdd(clmProductId, string.Format("CLM - {0}", clmProfilesFormatted));
                products.TryAdd(eSignatureProductId, string.Format("eSignature - {0}", eSignProfilesFormatted));

                this.ViewBag.Products = products;
            }

            this.ViewBag.EmailAddress = this.RequestItemsService.EmailAddress;
        }
    }
}