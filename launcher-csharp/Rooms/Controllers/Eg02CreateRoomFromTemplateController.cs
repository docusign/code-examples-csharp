using System.Collections.Generic;
using System.Linq;
using DocuSign.Rooms.Api;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Model;
using eg_03_csharp_auth_code_grant_core.Controllers;
using eg_03_csharp_auth_code_grant_core.Models;
using eg_03_csharp_auth_code_grant_core.Rooms.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Rooms.Controllers
{
    [Area("Rooms")]
    [Route("Eg02")]
    public class Eg02CreateRoomsFromTemplateController : EgController
    {
        private readonly IRoomsApi _roomsApi;
        private readonly IRolesApi _rolesApi;
        private readonly IRoomTemplatesApi _roomTemplatesApi;

        public Eg02CreateRoomsFromTemplateController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService,
            IRoomsApi roomsApi,
            IRolesApi rolesApi,
            IRoomTemplatesApi roomTemplatesApi) : base(dsConfig, requestItemsService)
        {
            _roomsApi = roomsApi;
            _rolesApi = rolesApi;
            _roomTemplatesApi = roomTemplatesApi;

            // Step 1. Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = RequestItemsService.Session.RoomsApiBasePath + "/restapi"; // Base API path

            // Step 2: Construct your API headers
            ConstructApiHeaders(accessToken, basePath);
        }

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
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {    //Step 3: Get Templates
                var templates = _roomTemplatesApi.GetRoomTemplates(accountId);

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
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Step 4: Obtain Role 
            RoleSummary clientRole = _rolesApi.GetRoles(accountId, new RolesApi.GetRolesOptions { filter = "Default Admin" }).Roles.First();

            // Step 5: Construct the request body for your room with using selected template Id
            RoomForCreate newRoom = BuildRoom(model, clientRole, model.TemplateId);

            try
            {
                // Step 6: Call the Rooms API to create a room
                var room = _roomsApi.CreateRoom(accountId, newRoom);

                ViewBag.h1 = "The room was successfully created";
                ViewBag.message = $"The room was created! Room ID: {room.RoomId}, name:{room.Name}.";
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

        private void ConstructApiHeaders(string accessToken, string basePath)
        {
            var config = new Configuration(new ApiClient(basePath));
            config.AddDefaultHeader("Authorization", "Bearer " + accessToken);
            _roomsApi.Configuration = config;
            _rolesApi.Configuration = config;
            _roomTemplatesApi.Configuration = config;
        }
    }
}