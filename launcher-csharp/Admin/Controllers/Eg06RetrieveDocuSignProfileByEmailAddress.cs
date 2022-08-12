// <copyright file="Eg06RetrieveDocuSignProfileByEmailAddress.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Admin.Controllers
{
    using System;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Examples;
    using DocuSign.Admin.Model;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Admin")]
    [Route("Aeg06")]
    public class Eg06RetrieveDocuSignProfileByEmailAddress : EgController
    {
        public Eg06RetrieveDocuSignProfileByEmailAddress(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgNumber);
            this.ViewBag.title = this.CodeExampleText.PageTitle;
        }

        public override int EgNumber => 6;

        public override string EgName => "Aeg06";

        [MustAuthenticate]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RetriveProfileByEmail(string email)
        {
            try
            {
                Guid? organizationId = this.RequestItemsService.OrganizationId;
                string accessToken = this.RequestItemsService.User.AccessToken;
                string basePath = this.RequestItemsService.Session.AdminApiBasePath;

                UsersDrilldownResponse profileWithSearchedEmail = RetrieveDocuSignProfileByEmailAddress.
                    GetDocuSignProfileByEmailAdress(basePath, accessToken, organizationId, email);

                this.ViewBag.h1 = this.CodeExampleText.ResultsPageHeader;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(profileWithSearchedEmail, Formatting.Indented);

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