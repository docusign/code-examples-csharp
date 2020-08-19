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
    [Route("Eg03")]
    public class Eg03ExportDataFromRoomController : EgController
    {
        private readonly IRoomsApi _roomsApi;

        public Eg03ExportDataFromRoomController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService,
            IRoomsApi roomsApi) : base(dsConfig, requestItemsService)
        {
            _roomsApi = roomsApi;

            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            string basePath = RequestItemsService.Session.RoomsApiBasePath + "/restapi"; // Base API path

            // Step 2: Construct your API headers
            ConstructApiHeaders(accessToken, basePath);
        }

        public override string EgName => "Eg03";

        [BindProperty]
        public RoomsListModel RoomsListModel { get; set; }

        protected override void InitializeInternal()
        {
            RoomsListModel = new RoomsListModel();
        }

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                //Step 3: Get Rooms
                RoomSummaryList rooms = _roomsApi.GetRooms(accountId);

                RoomsListModel = new RoomsListModel {Rooms = rooms.Rooms.ToList()};

                return View("Eg03", this);
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
        public ActionResult ExportData(RoomsListModel model)
        { 
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Step 4: Call the Rooms API to get room field data
                FieldData fieldData = _roomsApi.GetRoomFieldData(accountId, model.RoomId);
                
                ViewBag.h1 = "The room data was successfully exported";
                ViewBag.message = $"Results from the Rooms::GetRoomFieldData method RoomId: {model.RoomId} :";
                ViewBag.Locals.Json = JsonConvert.SerializeObject(fieldData, Formatting.Indented);

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
        }
    }
}