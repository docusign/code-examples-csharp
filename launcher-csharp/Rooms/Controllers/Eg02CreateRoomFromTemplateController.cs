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
            ViewBag.title = "Create room from template";
            _roomsApi = roomsApi;
            _rolesApi = rolesApi;
            _roomTemplatesApi = roomTemplatesApi;
        }

        public override string EgName => "Eg02";

        [BindProperty]
        public RoomModel RoomModel { get; set; }

        protected override void InitializeInternal()
        {
            RoomModel = new RoomModel();
        }

        [HttpGet]
        public override IActionResult Get()
        {
            // Check the token with minimal buffer time.
            var tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            // Step 1. Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            var basePath = RequestItemsService.Session.RoomsApiBasePath + "/restapi"; // Base API path

            // Step 2: Construct your API headers
            ConstructApiHeaders(accessToken, basePath);

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

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoomModel model)
        {
            // Check the token with minimal buffer time.
            var tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            // Step 1. Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            var basePath = RequestItemsService.Session.RoomsApiBasePath + "/restapi"; // Base API path

            // Step 2: Construct your API headers
            ConstructApiHeaders(accessToken, basePath);

            // Step 3: Obtain Role 
            RoleSummary clientRole = _rolesApi.GetRoles(accountId, new RolesApi.GetRolesOptions { filter = "Client" }).Roles.First();

            // Step 4: Construct the request body for your room with using selected template Id
            RoomForCreate newRoom = BuildRoom(model, clientRole, model.TemplateId);

            try
            {
                // Step 5. Call the Rooms API
                var room = _roomsApi.CreateRoom(accountId, newRoom);

                ViewBag.h1 = "The room was created";
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
                        {"address1", "123 EZ Street"},
                        {"address2", "unit 10"},
                        {"city", "New York"},
                        {"postalCode", "11112"},
                        {"companyRoomStatus", "5"},
                        {"state", "US-CA"},
                        {
                            "comments", @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, 
sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. 
Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
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