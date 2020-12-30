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
    [Route("Eg07")]
    public class Eg07CreateFormGroupController : EgController
    {
        public Eg07CreateFormGroupController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService) : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg07";

        [BindProperty]
        public FormGroupModel FormGroupModel { get; set; }

        protected override void InitializeInternal()
        {
            FormGroupModel = new FormGroupModel();
        }

        [MustAuthenticate]
        [Route("CreateFormGroup")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFormGroup(FormGroupModel formGroupModel)
        {
            // Step 1. Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2: Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var formGroupsApi = new FormGroupsApi(apiClient);
            string accountId = RequestItemsService.Session.AccountId;

            try
            {
                // Step 3: Call the Rooms API to create a form group
                FormGroup formGroup = formGroupsApi.CreateFormGroup(accountId, new FormGroupForCreate(formGroupModel.Name));

                ViewBag.h1 = "The form group was successfully created";
                ViewBag.message = $"The form group was successfully created, FormGroupId: '{formGroup.FormGroupId}'";
                ViewBag.Locals.Json = JsonConvert.SerializeObject(formGroup, Formatting.Indented);

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