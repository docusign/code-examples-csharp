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
    [Route("[area]/Eg01")]
    public class Eg01CreateClickwrapController : EgController
    {
        public Eg01CreateClickwrapController(
            DSConfiguration dsConfig, 
            IRequestItemsService requestItemsService) 
            : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg01";

        [MustAuthenticate]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string name)
        {
            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; 
            var basePath = $"{RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = RequestItemsService.Session.AccountId;

            try
            {
                // Call the Click API to create a clickwrap
                var clickWrap = CreateClickwrap.Create(name, basePath, accessToken, accountId);

                //Show results
                ViewBag.h1 = "The clickwrap was successfully created";
                ViewBag.message = $"The clickwrap was created! Clickwrap ID: {clickWrap.ClickwrapId}, Name: {clickWrap.ClickwrapName}.";
                ViewBag.Locals.Json = JsonConvert.SerializeObject(clickWrap, Formatting.Indented);

                // Save for future use within the example launcher
                RequestItemsService.ClickwrapId = clickWrap.ClickwrapId;

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