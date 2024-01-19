// <copyright file="CreateExternalFormFillSession.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    using System;
    using System.Collections.Generic;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.CodeExamples.Rooms.Models;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Model;
    using Microsoft.AspNetCore.Mvc;

    [Area("Rooms")]
    [Route("reg006")]
    public class CreateExternalFormFillSession : EgController
    {
        public CreateExternalFormFillSession(DsConfiguration dsConfig, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Rooms);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "reg006";

        [BindProperty]
        public RoomDocumentModel RoomDocumentModel { get; set; }

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
                // Get Rooms
                var rooms = DocuSign.Rooms.Examples.CreateExternalFormFillSession.GetRooms(basePath, accessToken, accountId);

                this.RoomDocumentModel = new RoomDocumentModel { Rooms = rooms.Rooms };

                base.InitializeInternal();
                return this.View("reg006", this);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectRoom(RoomDocumentModel roomDocumentModel)
        {
            // Step 1. Obtain your OAuth token
            string accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{this.RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Get Room Documents
                var documents = DocuSign.Rooms.Examples.CreateExternalFormFillSession.GetDocuments(basePath, accessToken, accountId, roomDocumentModel.RoomId);

                this.RoomDocumentModel.Documents = documents.Documents;

                base.InitializeInternal();
                return this.View("reg006", this);
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
        [Route("ExportData")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportData(RoomDocumentModel roomDocumentModel)
        {
            // Obtain your OAuth token
            string accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{this.RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                var formIds = new List<Guid?> { roomDocumentModel.DocumentId };
                // Call the Rooms API to create external form fill session
                var externalFormFillSession = DocuSign.Rooms.Examples.CreateExternalFormFillSession.CreateSession(basePath,
                    accessToken,
                    accountId,
                    new ExternalFormFillSessionForCreate(null, roomDocumentModel.RoomId, formIds, "https://localhost:44333"));

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                //ds-snippet-start:Rooms6Step5
                this.ViewBag.Url = externalFormFillSession.Url;
                this.ViewBag.JsonUrl = externalFormFillSession.ToJson();

                return this.View("embed");
                //ds-snippet-end:Rooms6Step5
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
            this.RoomDocumentModel = new RoomDocumentModel();
        }
    }
}