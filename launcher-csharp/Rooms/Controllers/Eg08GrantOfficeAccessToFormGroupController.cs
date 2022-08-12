// <copyright file="Eg08GrantOfficeAccessToFormGroupController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.CodeExamples.Rooms.Models;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Examples;
    using DocuSign.Rooms.Model;
    using Microsoft.AspNetCore.Mvc;

    [Area("Rooms")]
    [Route("Eg08")]
    public class Eg08GrantOfficeAccessToFormGroupController : EgController
    {
        public override int EgNumber => 8;

        public Eg08GrantOfficeAccessToFormGroupController(
            DSConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgNumber);
            this.ViewBag.title = this.CodeExampleText.PageTitle;
        }

        public override string EgName => "Eg08";

        [BindProperty]
        public OfficeAccessModel OfficeAccessModel { get; set; }

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            base.Get();

            // Obtain your OAuth token
            string accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{this.RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to get offices and form groups
                (OfficeSummaryList offices, FormGroupSummaryList formGroups) =
                    GrantOfficeAccessToFormGroup.GetOfficesAndFormGroups(basePath, accessToken, accountId);

                this.OfficeAccessModel = new OfficeAccessModel { Offices = offices.OfficeSummaries, FormGroups = formGroups.FormGroups };

                return this.View("Eg08", this);
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;

                return this.View("Error");
            }
        }

        [MustAuthenticate]
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
                GrantOfficeAccessToFormGroup.GrantAccess(basePath, accessToken, accountId, roomDocumentModel.FormGroupId, roomDocumentModel.OfficeId);

                this.ViewBag.h1 = this.CodeExampleText.ResultsPageHeader;
                this.ViewBag.message = string.Format(
                    this.CodeExampleText.ResultsPageText,
                    roomDocumentModel.OfficeId,
                    roomDocumentModel.FormGroupId);

                return this.View("example_done");
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;

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
