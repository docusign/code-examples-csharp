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
            string accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2 start
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            // Step 2 end
            
            var officesApi = new OfficesApi(apiClient);
            var formGroupsApi = new FormGroupsApi(apiClient);

            string accountId = RequestItemsService.Session.AccountId;

            try
            {
                // Step 3 start
                var offices = officesApi.GetOffices(accountId);
                // Step 3 end
                
                // Step 4 start
                var formGroups = formGroupsApi.GetFormGroups(accountId);
                // Step 4 end
                
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
            string accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var formGroupsApi = new FormGroupsApi(apiClient);

            string accountId = RequestItemsService.Session.AccountId;

            try
            {
                // Step 5 start
                formGroupsApi.GrantOfficeAccessToFormGroup(accountId, new Guid(roomDocumentModel.FormGroupId), roomDocumentModel.OfficeId);
                // Step 5 end
                
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
