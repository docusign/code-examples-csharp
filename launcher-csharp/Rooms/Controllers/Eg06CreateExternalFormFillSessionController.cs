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
    [Route("Eg06")]
    public class Eg06CreateExternalFormFillSessionController : EgController
    {
        private readonly IRoomsApi _roomsApi;
        private readonly IFormLibrariesApi _formLibrariesApi;
        private readonly IExternalFormFillSessionsApi _externalFormFillSessionsApi;

        public Eg06CreateExternalFormFillSessionController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService,
            IRoomsApi roomsApi,
            IFormLibrariesApi formLibrariesApi,
            IExternalFormFillSessionsApi externalFormFillSessionsApi) : base(dsConfig, requestItemsService)
        {
            _roomsApi = roomsApi;
            _formLibrariesApi = formLibrariesApi;
            _externalFormFillSessionsApi = externalFormFillSessionsApi;
        }

        public override string EgName => "Eg06";

        [BindProperty]
        public RoomDocumentModel RoomDocumentModel { get; set; }

        protected override void InitializeInternal()
        {
            RoomDocumentModel = new RoomDocumentModel();
        }

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2: Construct your API headers
            ConstructApiHeaders(accessToken, basePath);

            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                //Step 3: Get Rooms 
                RoomSummaryList rooms = _roomsApi.GetRooms(accountId);

                RoomDocumentModel = new RoomDocumentModel { Rooms = rooms.Rooms };

                return View("Eg06", this);
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }

        [MustAuthenticate]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectRoom(RoomDocumentModel roomDocumentModel)
        {
            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2: Construct your API headers
            ConstructApiHeaders(accessToken, basePath);

            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                //Step 3: Get Room Documents
                RoomDocumentList documents = _roomsApi.GetDocuments(accountId, roomDocumentModel.RoomId);

                RoomDocumentModel.Documents = documents.Documents;

                return View("Eg06", this);
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
        public ActionResult ExportData(RoomDocumentModel roomDocumentModel)
        {
            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2: Construct your API headers
            ConstructApiHeaders(accessToken, basePath);

            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Step 3: Call the Rooms API to create external form fill session
                ExternalFormFillSession url = _externalFormFillSessionsApi.CreateExternalFormFillSession(
                    accountId,
                    new ExternalFormFillSessionForCreate(roomDocumentModel.DocumentId.ToString(), roomDocumentModel.RoomId));

                ViewBag.h1 = "External form fill sessions was successfully created";
                ViewBag.message = $"To fill the form, navigate following URL: <a href='{url.Url}' target='_blank'>Fill the form</a>";
                ViewBag.Locals.Json = JsonConvert.SerializeObject(url, Formatting.Indented);

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
            _externalFormFillSessionsApi.Configuration = config;
        }
    }
}