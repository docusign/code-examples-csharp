// <copyright file="Eg01CreateRoomWithDataController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Examples;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Rooms")]
    [Route("Eg01")]
    public class CreateRoomWithData : EgController
    {
        public CreateRoomWithData(
            DSConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgNumber);
            this.ViewBag.title = this.CodeExampleText.PageTitle;
        }

        public override int EgNumber => 1;

        public override string EgName => "Eg01";

        [BindProperty]
        public Models.RoomModel RoomModel { get; set; }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            this.RoomModel = new Models.RoomModel();
        }

        [MustAuthenticate]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.RoomModel model)
        {
            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{this.RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Mapping room model to match types
                var mappedRoomModel = new DocuSign.Rooms.Examples.CreateRoomWithData.RoomModel
                {
                    Name = model.Name,
                    TemplateId = model.TemplateId,
                    Templates = model.Templates,
                };

                //  Call the Rooms API to create a room
                var room = DocuSign.Rooms.Examples.CreateRoomWithData.CreateRoom(basePath, accessToken, accountId, mappedRoomModel);

                // Show results
                this.ViewBag.h1 = this.CodeExampleText.ResultsPageHeader;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, room.RoomId.ToString(), room.Name);
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(room, Formatting.Indented);

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