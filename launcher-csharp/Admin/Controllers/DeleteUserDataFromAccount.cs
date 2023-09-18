// <copyright file="DeleteUserDataFromAccount.cs" company="DocuSign">
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
    [Route("Aeg011")]
    public class DeleteUserDataFromAccount : EgController
    {
        public DeleteUserDataFromAccount(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Admin);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Aeg011";

        [MustAuthenticate]
        [SetViewBag]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserDataFromAccountByUserId(string userId)
        {
            try
            {
                string accessToken = this.RequestItemsService.User.AccessToken;
                string basePath = this.RequestItemsService.Session.AdminApiBasePath;
                Guid accountId = Guid.Parse(this.RequestItemsService.Session.AccountId);

                IndividualUserDataRedactionResponse response =
                    DocuSign.Admin.Examples.DeleteUserDataFromAccount.DeleteUserDataFromAccountByUserId(
                        basePath,
                        accessToken,
                        accountId,
                        userId);

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