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
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path

            // Step 2 start
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            // Step 2 end
            
            var formGroupsApi = new FormGroupsApi(apiClient);
            string accountId = RequestItemsService.Session.AccountId;

            try
            {
                // Step 3 start
                var formGroupForCreate = new FormGroupForCreate(formGroupModel.Name);
                // Step 3 end                
                
                // Step 4 start
                FormGroup formGroup = formGroupsApi.CreateFormGroup(accountId, formGroupForCreate);
                // Step 4 end
                
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
