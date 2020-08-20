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
    [Route("Eg01")]
    public class Eg01CreateRoomWithDataController : EgController
    {
        private readonly IRoomsApi _roomsApi;
        private readonly IRolesApi _rolesApi;

        public Eg01CreateRoomWithDataController(
            DSConfiguration dsConfig, 
            IRequestItemsService requestItemsService,
            IRoomsApi roomsApi,
            IRolesApi rolesApi) : base(dsConfig, requestItemsService)
        {
            _roomsApi = roomsApi;
            _rolesApi = rolesApi;

            // Step 1. Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2: Construct your API headers
            ConstructApiHeaders(accessToken, basePath);
        }

        public override string EgName => "Eg01";

        [BindProperty]
        public RoomModel RoomModel { get; set; }

        protected override void InitializeInternal()
        {
            RoomModel = new RoomModel();
        }

        [MustAuthenticate]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoomModel model)
        {
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Step 3: Obtain Role 
                RoleSummary clientRole = _rolesApi.GetRoles(accountId, new RolesApi.GetRolesOptions { filter = "Default Admin" }).Roles.First();

                // Step 4: Construct the request body for your room
                RoomForCreate newRoom = BuildRoom(model, clientRole);

                // Step 5: Call the Rooms API to create a room
                Room room = _roomsApi.CreateRoom(accountId, newRoom);

                ViewBag.h1 = "The room was successfully created";
                ViewBag.message = $"The room was created! Room ID: {room.RoomId}, Name: {room.Name}.";

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }

        private static RoomForCreate BuildRoom(RoomModel model, RoleSummary clientRole)
        {
            var newRoom = new RoomForCreate
            {
                Name = model.Name,
                RoleId = clientRole.RoleId,
                TransactionSideId = "buy",
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
        }
    }
}