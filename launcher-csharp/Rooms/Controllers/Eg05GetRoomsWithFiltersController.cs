// <copyright file="Eg05GetRoomsWithFiltersController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    using System.Globalization;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.CodeExamples.Rooms.Models;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Examples;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Rooms")]
    [Route("Eg05")]
    public class Eg05GetRoomsWithFiltersController : EgController
    {
        public Eg05GetRoomsWithFiltersController(
            DSConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgNumber);
            this.ViewBag.title = this.CodeExampleText.PageTitle;
        }

        public override int EgNumber => 5;

        public override string EgName => "Eg05";

        [BindProperty]
        public RoomFilterModel RoomFilterModel { get; set; }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            this.RoomFilterModel = new RoomFilterModel();
        }

        [MustAuthenticate]
        [Route("ExportData")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportData(RoomFilterModel roomFilterModel)
        {
            // Obtain your OAuth token
            string accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{this.RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Prepare your request parameters
                var fieldDataChangedStartDate = roomFilterModel.FieldDataChangedStartDate.ToString(CultureInfo.InvariantCulture);
                var fieldDataChangedEndDate = roomFilterModel.FieldDataChangedEndDate.ToString(CultureInfo.InvariantCulture);

                // Call the Rooms API to get rooms with filters
                var rooms = GetRoomsWithFilters.GetRooms(basePath,
                    accessToken,
                    accountId,
                    fieldDataChangedStartDate,
                    fieldDataChangedEndDate);

                this.ViewBag.h1 = this.CodeExampleText.ResultsPageHeader;
                this.ViewBag.message = string.Format(
                    this.CodeExampleText.ResultsPageText,
                    roomFilterModel.FieldDataChangedStartDate.Date.ToShortDateString(),
                    roomFilterModel.FieldDataChangedEndDate.Date.ToShortDateString());
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(rooms, Formatting.Indented);

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