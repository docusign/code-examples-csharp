using System;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Rooms.Models;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Examples;
using DocuSign.Rooms.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ApiError = DocuSign.Rooms.Model.ApiError;

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    [Area("Rooms")]
    [Route("Eg04")]
    public class Eg04AddingFormToRoomController : EgController
    {
        public Eg04AddingFormToRoomController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService) : base(dsConfig, requestItemsService)
        {
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
            base.Get();
            // Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Get Rooms and forms
                (FormSummaryList forms, RoomSummaryList rooms) = AddingFormToRoom.GetFormsAndRooms(basePath, accessToken, accountId);

                RoomFormModel = new RoomFormModel { Forms = forms.Forms, Rooms = rooms.Rooms };

                return View("Eg04", this);
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                ApiError error = JsonConvert.DeserializeObject<ApiError>(apiException.ErrorContent);
                if (error.ErrorCode.Equals("FORMS_INTEGRATION_NOT_ENABLED", StringComparison.InvariantCultureIgnoreCase))
                {
                    return View("ExampleNotAvailable");
                }
                return View("Error");
            }
        }

        [MustAuthenticate]
        [Route("ExportData")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportData(RoomFormModel roomFormModel)
        {     
            // Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to add form to a room
                var roomDocument = AddingFormToRoom.AddForm(basePath,
                    accessToken,
                    accountId,
                    roomFormModel.RoomId,
                    roomFormModel.FormId);

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
    }
}