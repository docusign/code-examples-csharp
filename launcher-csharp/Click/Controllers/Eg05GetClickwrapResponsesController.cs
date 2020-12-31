using DocuSign.Click.Api;
using DocuSign.Click.Client;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Click.Controllers
{
    [Area("Click")]
    [Route("[area]/Eg07")]
    public class Eg07GetClickwrapResponsesController : EgController
    {
        public Eg07GetClickwrapResponsesController(
            DSConfiguration dsConfig, 
            IRequestItemsService requestItemsService) 
            : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg07";
        protected override void InitializeInternal()
        {
            ViewBag.ClickwrapId = RequestItemsService.ClickwrapId;
            ViewBag.AccountId = RequestItemsService.Session.AccountId;
        }
    
    [MustAuthenticate]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create()
        {
            // Step 1. Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.BasePath}/clickapi"; // Base API path

            // Step 2: Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(apiClient);

            try
            {
                if (string.IsNullOrEmpty(RequestItemsService.ClickwrapId))
                {
                    ViewBag.errorCode = 400;
                    ViewBag.errorMessage = "Cannot find any clickwrap. Please first create a clickwrap using the first example.";

                    return View("Error");
                }

                var accountId = RequestItemsService.Session.AccountId;
                var clickwrapId = RequestItemsService.ClickwrapId;

                // Step 3: Call the Click API to get a clickwrap agreements
                var clickWrapAgreements = clickAccountApi.GetClickwrapAgreements(accountId, clickwrapId);

                //Show results
                ViewBag.h1 = "The data was successfully fetched";
                ViewBag.message = $"The clickwrap contains {clickWrapAgreements.UserAgreements.Count} agreements.";
                ViewBag.Locals.Json = JsonConvert.SerializeObject(clickWrapAgreements, Formatting.Indented);

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