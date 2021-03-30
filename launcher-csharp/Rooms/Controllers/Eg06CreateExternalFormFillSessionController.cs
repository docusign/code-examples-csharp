using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Rooms.Models;
using DocuSign.Rooms.Api;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Examples;
using DocuSign.Rooms.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    [Area("Rooms")]
    [Route("Eg06")]
    public class Eg06CreateExternalFormFillSessionController : EgController
    {
        public Eg06CreateExternalFormFillSessionController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService) : base(dsConfig, requestItemsService)
        {
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
            base.Get();

            // Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Get Rooms 
                var rooms = CreateExternalFormFillSession.GetRooms(basePath, accessToken, accountId);

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
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Get Room Documents
                var documents = CreateExternalFormFillSession.GetDocuments(basePath, accessToken, accountId, roomDocumentModel.RoomId);

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
            // Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to create external form fill session
                var url = CreateExternalFormFillSession.CreateSession(basePath,
                    accessToken,
                    accountId,
                    new ExternalFormFillSessionForCreate(roomDocumentModel.DocumentId.ToString(),
                        roomDocumentModel.RoomId));

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
    }
}