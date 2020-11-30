using System.Linq;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Rooms.Models;
using DocuSign.Rooms.Api;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    [Area("Rooms")]
    [Route("Eg03")]
    public class Eg03ExportDataFromRoomController : EgController
    {
        public Eg03ExportDataFromRoomController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService) : base(dsConfig, requestItemsService)
        {
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
            base.Get();
            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2: Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var roomsApi = new RoomsApi(apiClient);

            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                //Step 3: Get Rooms
                RoomSummaryList rooms = roomsApi.GetRooms(accountId);

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
            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2: Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var roomsApi = new RoomsApi(apiClient);

            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Step 3: Call the Rooms API to get room field data
                FieldData fieldData = roomsApi.GetRoomFieldData(accountId, model.RoomId);
                
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
    }
}