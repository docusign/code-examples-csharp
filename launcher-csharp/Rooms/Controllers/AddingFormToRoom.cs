// <copyright file="AddingFormToRoom.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
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
    [Route("reg004")]
    public class AddingFormToRoom : EgController
    {
        public AddingFormToRoom(
            DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Rooms);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "reg004";

        [BindProperty]
        public RoomFormModel RoomFormModel { get; set; }

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
                // Get Rooms and forms
                (FormSummaryList forms, RoomSummaryList rooms) = DocuSign.Rooms.Examples.AddingFormToRoom.GetFormsAndRooms(basePath, accessToken, accountId);

                this.RoomFormModel = new RoomFormModel { Forms = forms.Forms, Rooms = rooms.Rooms };

                return this.View("reg004", this);
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                ApiError error = JsonConvert.DeserializeObject<ApiError>(apiException.ErrorContent);
                if (error.ErrorCode.Equals(this.CodeExampleText.CustomErrorTexts[0].ErrorMessageCheck, StringComparison.InvariantCultureIgnoreCase))
                {
                    return this.View(this.CodeExampleText.CustomErrorTexts[0].ErrorMessage);
                }

                return this.View("Error");
            }
        }

        [MustAuthenticate]
        [SetViewBag]
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
                var roomDocument = DocuSign.Rooms.Examples.AddingFormToRoom.AddForm(basePath,
                    accessToken,
                    accountId,
                    roomFormModel.RoomId,
                    roomFormModel.FormId);

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText + $"<br/> RoomId: {roomFormModel.RoomId}, FormId: {roomFormModel.FormId} :";
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(roomDocument, Formatting.Indented);

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
            this.RoomFormModel = new RoomFormModel();
        }
    }
}