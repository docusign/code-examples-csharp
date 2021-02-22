using DocuSign.Click.Client;
using DocuSign.Click.Examples;
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
            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = RequestItemsService.Session.AccountId;

            try
            {
                if (string.IsNullOrEmpty(RequestItemsService.ClickwrapId))
                {
                    ViewBag.errorCode = 400;
                    ViewBag.errorMessage = "Cannot find any clickwrap. Please first create a clickwrap using the first example.";

                    return View("Error");
                }

                var clickwrapId = RequestItemsService.ClickwrapId;
                var clickwrapVersion = "1"; // A newly created clickwrap has default version 1

                // Call the Click API to activate a clickwrap
                var clickWrap = ActivateClickwrap.Update(clickwrapId, clickwrapVersion, basePath, accessToken, accountId);

                // Show results
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
    }
}