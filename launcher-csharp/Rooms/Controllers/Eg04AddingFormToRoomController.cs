using System.Collections.Generic;
using System.Linq;
using DocuSign.Rooms.Api;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Model;
using eg_03_csharp_auth_code_grant_core.Controllers;
using eg_03_csharp_auth_code_grant_core.Models;
using eg_03_csharp_auth_code_grant_core.Rooms.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eg_03_csharp_auth_code_grant_core.Rooms.Controllers
{
    [Area("Rooms")]
    [Route("Eg04")]
    public class Eg04AddingFormToRoomController : EgController
    {
        private readonly IRoomsApi _roomsApi;
        private readonly IFormLibrariesApi _formLibrariesApi;

        public Eg04AddingFormToRoomController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService,
            IRoomsApi roomsApi,
            IFormLibrariesApi formLibrariesApi) : base(dsConfig, requestItemsService)
        {
            _roomsApi = roomsApi;
            _formLibrariesApi = formLibrariesApi;
        }

        public override string EgName => "Eg04";

        [BindProperty]
        public RoomFormModel RoomFormModel { get; set; }

        protected override void InitializeInternal()
        {
            RoomFormModel = new RoomFormModel();
        }

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            string basePath = RequestItemsService.Session.RoomsApiBasePath + "/restapi"; // Base API path

            // Step 2: Construct your API headers
            ConstructApiHeaders(accessToken, basePath);

            try
            {
                //Step 3: Get Forms Libraries
                FormLibrarySummaryList formLibraries = _formLibrariesApi.GetFormLibraries(accountId);

                //Step 4: Get Forms 
                FormSummaryList forms = new FormSummaryList(new List<FormSummary>());
                if (formLibraries.FormsLibrarySummaries.Any())
                {
                    forms = _formLibrariesApi.GetFormLibraryForms(
                        accountId,
                        formLibraries.FormsLibrarySummaries.First().FormsLibraryId);
                }

                //Step 5: Get Rooms 
                RoomSummaryList rooms = _roomsApi.GetRooms(accountId);

                RoomFormModel = new RoomFormModel { Forms = forms.Forms, Rooms = rooms.Rooms };

                return View("Eg04", this);
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }

        [MustAuthenticate]
        [Route("ExportData")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportData(RoomFormModel roomFormModel)
        {
            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            string basePath = RequestItemsService.Session.RoomsApiBasePath + "/restapi"; // Base API path

            // Step 2: Construct your API headers
            ConstructApiHeaders(accessToken, basePath);

            try
            {
                // Step 3. Call the Rooms API 
                RoomDocument roomDocument = _roomsApi.AddFormToRoom(
                    accountId, 
                    roomFormModel.RoomId, 
                    new FormForAdd(roomFormModel.FormId));
               
                ViewBag.h1 = "The form was successfully added to a room";
                ViewBag.message = $"Results from the Rooms: AddFormToRoom method. RoomId: {roomFormModel.RoomId}, FormId: {roomFormModel.FormId} :";
                ViewBag.Locals.Json = JsonConvert.SerializeObject(roomDocument, Formatting.Indented);

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }

        private void ConstructApiHeaders(string accessToken, string basePath)
        {
            var config = new Configuration(new ApiClient(basePath));
            config.AddDefaultHeader("Authorization", "Bearer " + accessToken);
            
            _roomsApi.Configuration = config;
            _formLibrariesApi.Configuration = config;
        }
    }
}