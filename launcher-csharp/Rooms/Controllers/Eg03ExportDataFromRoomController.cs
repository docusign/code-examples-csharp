using System.Linq;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Rooms.Models;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Examples;
using Microsoft.AspNetCore.Mvc;
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

            // Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Get Rooms
                var rooms = ExportDataFromRoom.GetRooms(basePath, accessToken, accountId);

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
            // Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to get room field data
                var fieldData = ExportDataFromRoom.Export(basePath, accessToken, accountId, model.RoomId);

                // Show results
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