using DocuSign.Click.Api;
using DocuSign.Click.Client;
using DocuSign.Click.Model;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Click.Controllers
{
    [Area("Click")]
    [Route("[area]/Eg02")]
    public class Eg02ActivateClickwrapController : EgController
    {
        public Eg02ActivateClickwrapController(
            DSConfiguration dsConfig, 
            IRequestItemsService requestItemsService) 
            : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg02";

        protected override void InitializeInternal()
        {
            ViewBag.ClickwrapId = RequestItemsService.ClickwrapId;
            ViewBag.AccountId = RequestItemsService.Session.AccountId;
        }
    

    [MustAuthenticate]
        [Route("Activate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Activate()
        {
            // Step 1. Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.BasePath}/clickapi"; // Base API path

            // Step 2: Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(apiClient);

            var accountId = RequestItemsService.Session.AccountId;

            try
            {
                if (string.IsNullOrEmpty(RequestItemsService.ClickwrapId))
                {
                    ViewBag.errorCode = 400;
                    ViewBag.errorMessage = "Cannot find any clickwrap. Please first create a clickwrap using the first example.";

                    return View("Error");
                }

                // Step 3: Construct the request body for your clickwrap
                ClickwrapRequest clickwrapRequest = BuildUpdateClickwrapVersionRequest();

                var clickwrapId = RequestItemsService.ClickwrapId;
                var clickwrapVersion = "1"; //A newly created clickwrap has default version 1

                // Step 4: Call the Click API to activate a clickwrap
                var clickWrap = clickAccountApi.UpdateClickwrapVersion(accountId, clickwrapId, clickwrapVersion, clickwrapRequest);

                //Show results
                ViewBag.h1 = "The clickwrap was successfully activated";
                ViewBag.message = $"The clickwrap was activated! Clickwrap ID: {clickWrap.ClickwrapId}, Name: {clickWrap.ClickwrapName}.";
                ViewBag.Locals.Json = JsonConvert.SerializeObject(clickWrap, Formatting.Indented);

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }

        private static ClickwrapRequest BuildUpdateClickwrapVersionRequest()
        {
            var clickwrapRequest = new ClickwrapRequest
            {
                Status = "active"
            };

            return clickwrapRequest;
        }
    }
}