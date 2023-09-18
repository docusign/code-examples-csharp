// <copyright file="ExportDataFromRoom.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    using System.Linq;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.CodeExamples.Rooms.Models;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Examples;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Rooms")]
    [Route("reg003")]
    public class ExportDataFromRoom : EgController
    {
        public ExportDataFromRoom(
            DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Rooms);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "reg003";

        [BindProperty]
        public RoomsListModel RoomsListModel { get; set; }

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
                var rooms = DocuSign.Rooms.Examples.ExportDataFromRoom.GetRooms(basePath, accessToken, accountId);

                this.RoomsListModel = new RoomsListModel { Rooms = rooms.Rooms.ToList() };

                return this.View("reg003", this);
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
        public ActionResult ExportData(RoomsListModel model)
        {
            // Obtain your OAuth token
            string accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{this.RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to get room field data
                var fieldData = DocuSign.Rooms.Examples.ExportDataFromRoom.Export(basePath, accessToken, accountId, model.RoomId);

                // Show results
                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText + $"<br/>RoomId: {model.RoomId} :";
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(fieldData, Formatting.Indented);

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
            this.RoomsListModel = new RoomsListModel();
        }
    }
}