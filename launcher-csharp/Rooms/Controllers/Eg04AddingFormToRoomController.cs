// <copyright file="Eg04AddingFormToRoomController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    using System;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.CodeExamples.Rooms.Models;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Examples;
    using DocuSign.Rooms.Model;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using ApiError = DocuSign.Rooms.Model.ApiError;

    [Area("Rooms")]
    [Route("Eg04")]
    public class Eg04AddingFormToRoomController : EgController
    {
        public Eg04AddingFormToRoomController(
            DSConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgNumber);
            this.ViewBag.title = this.CodeExampleText.PageTitle;
        }

        public override int EgNumber => 4;

        public override string EgName => "Eg04";

        [BindProperty]
        public RoomFormModel RoomFormModel { get; set; }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            this.RoomFormModel = new RoomFormModel();
        }

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
                // Get Rooms and forms
                (FormSummaryList forms, RoomSummaryList rooms) = AddingFormToRoom.GetFormsAndRooms(basePath, accessToken, accountId);

                this.RoomFormModel = new RoomFormModel { Forms = forms.Forms, Rooms = rooms.Rooms };

                return this.View("Eg04", this);
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;

                ApiError error = JsonConvert.DeserializeObject<ApiError>(apiException.ErrorContent);
                if (error.ErrorCode.Equals("FORMS_INTEGRATION_NOT_ENABLED", StringComparison.InvariantCultureIgnoreCase))
                {
                    return this.View("ExampleNotAvailable");
                }

                return this.View("Error");
            }
        }

        [MustAuthenticate]
        [Route("ExportData")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportData(RoomFormModel roomFormModel)
        {
            // Obtain your OAuth token
            string accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{this.RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to add form to a room
                var roomDocument = AddingFormToRoom.AddForm(basePath,
                    accessToken,
                    accountId,
                    roomFormModel.RoomId,
                    roomFormModel.FormId);

                this.ViewBag.h1 = this.CodeExampleText.ResultsPageHeader;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText + $" RoomId: {roomFormModel.RoomId}, FormId: {roomFormModel.FormId} :";
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(roomDocument, Formatting.Indented);

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