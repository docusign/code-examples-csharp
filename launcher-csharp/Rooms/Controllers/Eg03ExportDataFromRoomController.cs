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
            ViewBag.title = "Export room data";
            _roomsApi = roomsApi;
        }

        public override string EgName => "Eg03";

        [BindProperty]
        public RoomsListViewModel RoomsListViewModel { get; set; }

        protected override void InitializeInternal()
        {
            RoomsListViewModel = new RoomsListViewModel();
        }

        [HttpGet]
        public override IActionResult Get()
        {
            // Check the token with minimal buffer time.
            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            string basePath = RequestItemsService.Session.RoomsApiBasePath + "/restapi"; // Base API path

            // Step 2: Construct your API headers
            ConstructApiHeaders(accessToken, basePath);

            try
            {
                //Step 3: Get Templates
                RoomSummaryList rooms = _roomsApi.GetRooms(accountId);

                RoomsListViewModel = new RoomsListViewModel {Rooms = rooms.Rooms.ToList()};

                return View("Eg03", this);
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;
                return View("Error");
            }
        }

        [Route("ExportData")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportData(RoomsListViewModel model)
        {
            // Check the token with minimal buffer time.
            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            string basePath = RequestItemsService.Session.RoomsApiBasePath + "/restapi"; // Base API path

            // Step 2: Construct your API headers
            ConstructApiHeaders(accessToken, basePath);

            try
            {
                // Step 5. Call the Rooms API 
                FieldData fieldData = _roomsApi.GetRoomFieldData(accountId, model.RoomId);
                ViewBag.h1 = "Export room data results";
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