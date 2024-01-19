// <copyright file="CloneAccount.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Admin.Controllers
{
    using System;
    using System.Linq;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Model;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Admin")]
    [Route("aeg012")]
    public class CloneAccount : EgController
    {
        public CloneAccount(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Admin);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "aeg012";

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            IActionResult actionResult = base.Get();
            if (this.RequestItemsService.EgName == this.EgName)
            {
                return actionResult;
            }

            Guid? organizationId = this.RequestItemsService.OrganizationId;
            string accessToken = this.RequestItemsService.User.AccessToken;
            string basePath = this.RequestItemsService.Session.AdminApiBasePath;

            AssetGroupAccountsResponse accounts = DocuSign.Admin.Examples.CloneAccount.GetGroupAccounts(basePath, accessToken, organizationId);
            this.ViewBag.Accounts = accounts.AssetGroupAccounts.ToList();

            return this.View(this.EgName, this);
        }

        [MustAuthenticate]
        [SetViewBag]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CloneAccountData(
            Guid? sourceAccountId,
            string targetAccountName,
            string targetAccountFirstName,
            string targetAccountLastName,
            string targetAccountEmail)
        {
            try
            {
                Guid? organizationId = this.RequestItemsService.OrganizationId;
                string accessToken = this.RequestItemsService.User.AccessToken;
                string basePath = this.RequestItemsService.Session.AdminApiBasePath;

                AssetGroupAccountClone response = DocuSign.Admin.Examples.CloneAccount.CloneGroupAccount(
                    basePath,
                    accessToken,
                    organizationId,
                    sourceAccountId,
                    targetAccountName,
                    targetAccountFirstName,
                    targetAccountLastName,
                    targetAccountEmail);

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
    }
}