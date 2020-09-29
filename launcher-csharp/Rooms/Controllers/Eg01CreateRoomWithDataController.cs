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
            // Step 1. Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2: Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var roomsApi = new RoomsApi(apiClient);
            var rolesApi = new RolesApi(apiClient);

            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Step 3: Obtain Role 
                RoleSummary clientRole = rolesApi.GetRoles(accountId, new RolesApi.GetRolesOptions { filter = "Default Admin" }).Roles.First();

                // Step 4: Construct the request body for your room
                RoomForCreate newRoom = BuildRoom(model, clientRole);

                // Step 5: Call the Rooms API to create a room
                Room room = roomsApi.CreateRoom(accountId, newRoom);

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
    }
}