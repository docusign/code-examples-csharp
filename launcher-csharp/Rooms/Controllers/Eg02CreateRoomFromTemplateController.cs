using System.Collections.Generic;
using System.Linq;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Rooms.Models;
using DocuSign.Rooms.Api;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Model;
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
            // Step 1. Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2: Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var roomTemplatesApi = new RoomTemplatesApi(apiClient);

            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {    //Step 3: Get Templates
                var templates = roomTemplatesApi.GetRoomTemplates(accountId);

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
            // Step 1. Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2: Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var roomsApi = new RoomsApi(apiClient);
            var rolesApi = new RolesApi(apiClient);

            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Step 4: Obtain Role 
            RoleSummary clientRole = rolesApi.GetRoles(accountId, new RolesApi.GetRolesOptions { filter = "Default Admin" }).Roles.First();

            // Step 5: Construct the request body for your room with using selected template Id
            RoomForCreate newRoom = BuildRoom(model, clientRole, model.TemplateId);

            try
            {
                // Step 6: Call the Rooms API to create a room
                Room room = roomsApi.CreateRoom(accountId, newRoom);

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

private static RoomForCreate BuildRoom(RoomModel model, RoleSummary clientRole, int? templateId)
{
    var newRoom = new RoomForCreate
    {
        Name = model.Name,
        RoleId = clientRole.RoleId, 
        TemplateId = templateId,
        FieldData = new FieldDataForCreate
        {
            Data = new Dictionary<string, object>
            {
                {"address1", "Street 1"},
                {"address2", "Unit 10"},
                {"city", "New York"},
                {"postalCode", "11112"},
                {"companyRoomStatus", "5"},
                {"state", "US-NY"},
                {
                    "comments", @"New room for sale."
                }
            }
        }
    };

    return newRoom;
}
    }
}