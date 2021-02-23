using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Examples;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    [Area("Rooms")]
    [Route("Eg01")]
    public class Eg01CreateRoomWithDataController : EgController
    {
        public Eg01CreateRoomWithDataController(
            DSConfiguration dsConfig, 
            IRequestItemsService requestItemsService) : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg01";

        [BindProperty]
        public Models.RoomModel RoomModel { get; set; }

        protected override void InitializeInternal()
        {
            RoomModel = new Models.RoomModel();
        }

        [MustAuthenticate]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.RoomModel model)
        {
            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Mapping room model to match types
                var mappedRoomModel = new CreateRoomWithData.RoomModel
                {
                    Name = model.Name,
                    TemplateId = model.TemplateId,
                    Templates = model.Templates
                };

                //  Call the Rooms API to create a room
                var room = CreateRoomWithData.CreateRoom(basePath, accessToken, accountId, mappedRoomModel);

                // Show results
                ViewBag.h1 = "The room was successfully created";
                ViewBag.message = $"The room was created! Room ID: {room.RoomId}, Name: {room.Name}.";
                ViewBag.Locals.Json = JsonConvert.SerializeObject(room, Formatting.Indented);

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }
    }
}