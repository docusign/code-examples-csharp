using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Rooms.Models;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Examples;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    [Area("Rooms")]
    [Route("Eg02")]
    public class Eg02CreateRoomFromTemplateController : EgController
    {

        public Eg02CreateRoomFromTemplateController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService) : base(dsConfig, requestItemsService)
        {}

        public override string EgName => "Eg02";

        [BindProperty]
        public RoomModel RoomModel { get; set; }

        protected override void InitializeInternal()
        {
            RoomModel = new RoomModel();
        }

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            base.Get();

            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {   
                // Get Templates
                var templates = CreateRoomFromTemplate.GetTemplates(basePath, accessToken, accountId);

                RoomModel = new RoomModel { Templates = templates.RoomTemplates };

                return View("Eg02", this);

            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;
                return View("Error");
            }
        }

        [MustAuthenticate]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoomModel model)
        {
            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Mapping room model to match types
                var mappedRoomModel = new CreateRoomFromTemplate.RoomModel
                {
                    Name = model.Name,
                    TemplateId = model.TemplateId,
                    Templates = model.Templates
                };

                // Call the Rooms API to create a room
                var room = CreateRoomFromTemplate.CreateRoom(basePath,
                    accessToken,
                    accountId,
                    mappedRoomModel,
                    mappedRoomModel.TemplateId);

                // Show results
                ViewBag.h1 = "The room was successfully created";
                ViewBag.message = $"The room was created! Room ID: {room.RoomId}, name:{room.Name}.";
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