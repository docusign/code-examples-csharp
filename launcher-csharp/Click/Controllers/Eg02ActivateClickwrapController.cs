using DocuSign.Click.Client;
using DocuSign.Click.Examples;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

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
            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = RequestItemsService.Session.AccountId;
            ViewBag.ClickwrapsData = ActivateClickwrap.GetInactiveClickwraps(basePath, accessToken, accountId);
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

                var clickwrapId = RequestItemsService.Clickwrap.ClickwrapId;
                var clickwrapVersion = RequestItemsService.Clickwrap.VersionNumber;

                // Call the Click API to activate a clickwrap
                var clickWrap = ActivateClickwrap.Update(clickwrapId, clickwrapVersion, basePath, accessToken, accountId);

                // Show results
                ViewBag.h1 = "Activate a clickwrap";
                ViewBag.message = $"The clickwrap {clickWrap.ClickwrapName} has been activated.";
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