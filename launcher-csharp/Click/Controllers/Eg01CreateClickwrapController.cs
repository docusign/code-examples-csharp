
using System;
using System.Collections.Generic;
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
                // Step 3: Construct the request body for your clickwrap
                ClickwrapRequest clickwrapRequest = BuildClickwraprequest(name);

                // Step 4: Call the Click API to create a clickwrap
                var clickWrap = clickAccountApi.CreateClickwrap(accountId, clickwrapRequest);

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

        private static ClickwrapRequest BuildClickwraprequest(string name)
        {
            var clickwrapRequest = new ClickwrapRequest
            {
                DisplaySettings = new DisplaySettings()
                {
                    ConsentButtonText = "I Agree",
                    DisplayName = "Terms of Service",
                    Downloadable = true,
                    Format = "modal",
                    MustRead = true,
                    MustView = true,
                    RequireAccept = true,
                    DocumentDisplay = "document"
                },
                Documents = new List<Document>(){
                    new Document()
                    {
                        DocumentBase64=Convert.ToBase64String(System.IO.File.ReadAllBytes("Terms_of_service.pdf")),
                        DocumentName="Terms of Service",
                        FileExtension="pdf",
                        Order= 0
                    }
                },
                Name = name,
                RequireReacceptance = true
            };

            return clickwrapRequest;
        }
    }
}