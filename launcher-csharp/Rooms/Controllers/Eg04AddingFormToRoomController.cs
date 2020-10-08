using System;
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
            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2: Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var roomsApi = new RoomsApi(apiClient);
            var formLibrariesApi = new FormLibrariesApi(apiClient);

            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                //Step 3: Get Forms Libraries
                FormLibrarySummaryList formLibraries = formLibrariesApi.GetFormLibraries(accountId);

                //Step 4: Get Forms 
                FormSummaryList forms = new FormSummaryList(new List<FormSummary>());
                if (formLibraries.FormsLibrarySummaries.Any())
                {
                    forms = formLibrariesApi.GetFormLibraryForms(
                        accountId,
                        formLibraries.FormsLibrarySummaries.First().FormsLibraryId);
                }

                //Step 5: Get Rooms 
                RoomSummaryList rooms = roomsApi.GetRooms(accountId);

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
                // Step 3: Call the Rooms API to add form to a room
                RoomDocument roomDocument = roomsApi.AddFormToRoom(
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
    }
}