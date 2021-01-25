using System;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Rooms.Models;
using DocuSign.Rooms.Api;
using DocuSign.Rooms.Client;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    [Area("Rooms")]
    [Route("Eg08")]
    public class Eg08GrantOfficeAccessToFormGroupController : EgController
    {
        public Eg08GrantOfficeAccessToFormGroupController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService) : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg08";

        [BindProperty]
        public OfficeAccessModel OfficeAccessModel { get; set; }

        protected override void InitializeInternal()
        {
            OfficeAccessModel = new OfficeAccessModel();
        }

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            base.Get();
            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2: Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var officesApi = new OfficesApi(apiClient);
            var formGroupsApi = new FormGroupsApi(apiClient);

            string accountId = RequestItemsService.Session.AccountId;

            try
            {
                //Step 3: Get Offices 
                var offices = officesApi.GetOffices(accountId);

                //Step 4: Get Form Groups 
                var formGroups = formGroupsApi.GetFormGroups(accountId);

                OfficeAccessModel = new OfficeAccessModel { Offices = offices.OfficeSummaries, FormGroups = formGroups.FormGroups };

                return View("Eg08", this);
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }

        [MustAuthenticate]
        [Route("GrantAccess")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GrantAccess(OfficeAccessModel roomDocumentModel)
        {
            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2: Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var formGroupsApi = new FormGroupsApi(apiClient);

            string accountId = RequestItemsService.Session.AccountId;

            try
            {
                // Step 4: Call the Rooms API to grant access
                formGroupsApi.GrantOfficeAccessToFormGroup(accountId, new Guid(roomDocumentModel.FormGroupId), roomDocumentModel.OfficeId);

                ViewBag.h1 = "Access is granted for the office";
                ViewBag.message = $"To the office with Id'{roomDocumentModel.OfficeId}' granted access for the form group with id '{roomDocumentModel.FormGroupId}'";

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