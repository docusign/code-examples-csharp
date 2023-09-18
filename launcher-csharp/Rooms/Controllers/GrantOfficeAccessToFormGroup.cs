// <copyright file="GrantOfficeAccessToFormGroup.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.CodeExamples.Rooms.Models;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Examples;
    using DocuSign.Rooms.Model;
    using Microsoft.AspNetCore.Mvc;

    [Area("Rooms")]
    [Route("reg008")]
    public class GrantOfficeAccessToFormGroup : EgController
    {
        public GrantOfficeAccessToFormGroup(
            DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Rooms);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "reg008";

        [BindProperty]
        public OfficeAccessModel OfficeAccessModel { get; set; }

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            IActionResult actionResult = base.Get();
            if (this.RequestItemsService.EgName == this.EgName)
            {
                return actionResult;
            }

            // Obtain your OAuth token
            string accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{this.RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to get offices and form groups
                (OfficeSummaryList offices, FormGroupSummaryList formGroups) =
                    DocuSign.Rooms.Examples.GrantOfficeAccessToFormGroup.GetOfficesAndFormGroups(basePath, accessToken, accountId);

                this.OfficeAccessModel = new OfficeAccessModel { Offices = offices.OfficeSummaries, FormGroups = formGroups.FormGroups };

                return this.View("reg008", this);
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }
        }

        [MustAuthenticate]
        [SetViewBag]
        [Route("GrantAccess")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GrantAccess(OfficeAccessModel roomDocumentModel)
        {
            // Obtain your OAuth token
            string accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{this.RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to grant office access to a form group
                DocuSign.Rooms.Examples.GrantOfficeAccessToFormGroup.GrantAccess(basePath, accessToken, accountId, roomDocumentModel.FormGroupId, roomDocumentModel.OfficeId);

                this.ViewBag.h1 = "Access is granted for the office";
                this.ViewBag.message = $"To the office with Id'{roomDocumentModel.OfficeId}' granted access for the form group with id '{roomDocumentModel.FormGroupId}'";

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
            this.OfficeAccessModel = new OfficeAccessModel();
        }
    }
}
