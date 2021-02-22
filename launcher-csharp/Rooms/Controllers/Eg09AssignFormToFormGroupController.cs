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

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    [Area("Rooms")]
    [Route("Eg09")]
    public class Eg09AssignFormToFormGroupController : EgController
    {
        public Eg09AssignFormToFormGroupController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService) : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg09";

        [BindProperty]
        public FormFormGroupModel FormFormGroupModel { get; set; }

        protected override void InitializeInternal()
        {
            FormFormGroupModel = new FormFormGroupModel();
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
            
            var formGroupsApi = new FormGroupsApi(apiClient);
            var formLibrariesApi = new FormLibrariesApi(apiClient);

            string accountId = RequestItemsService.Session.AccountId;

            try
            {
                // Step 3 start
                FormLibrarySummaryList formLibraries = formLibrariesApi.GetFormLibraries(accountId);

                FormSummaryList forms = new FormSummaryList(new List<FormSummary>());
                if (formLibraries.FormsLibrarySummaries.Any())
                {
                    forms = formLibrariesApi.GetFormLibraryForms(
                        accountId,
                        formLibraries.FormsLibrarySummaries.First().FormsLibraryId);
                }
                // Step 3 end
                
                // Step 4 start
                FormGroupSummaryList formGroups = formGroupsApi.GetFormGroups(accountId);
                // Step 4 end
                
                FormFormGroupModel = new FormFormGroupModel { Forms = forms.Forms, FormGroups = formGroups.FormGroups };

                return View("Eg09", this);
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }

        [MustAuthenticate]
        [Route("AssignFormToFormGroup")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignFormToFormGroup(FormFormGroupModel formFormGroupModel)
        {
            string accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var formGroupsApi = new FormGroupsApi(apiClient);

            string accountId = RequestItemsService.Session.AccountId; 

            try
            {
                // Step 6 start
                FormGroupFormToAssign formGroupFormToAssign = formGroupsApi.AssignFormGroupForm(accountId, new Guid(formFormGroupModel.FormGroupId), new FormGroupFormToAssign() { FormId = formFormGroupModel.FormId });
                // Step 6 end
                
                ViewBag.h1 = "The form was assigned to the form group successfully";
                ViewBag.message = $"The form ('{formGroupFormToAssign.FormId}') was assigned to the form group ('{formFormGroupModel.FormGroupId}') successfully";
                ViewBag.Locals.Json = JsonConvert.SerializeObject(formGroupFormToAssign, Formatting.Indented);

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
