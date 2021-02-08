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
    [Route("[area]/Eg03")]
    public class Eg03CreateNewClickwrapVersionController : EgController
    {
        public Eg03CreateNewClickwrapVersionController(
            DSConfiguration dsConfig, 
            IRequestItemsService requestItemsService) 
            : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg03";

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

            var accountId = RequestItemsService.Session.AccountId;

            try
            {
                if (string.IsNullOrEmpty(RequestItemsService.ClickwrapId))
                {
                    ViewBag.errorCode = 400;
                    ViewBag.errorMessage = "Cannot find any clickwrap. Please first create a clickwrap using the first example.";

                    return View("Error");
                }

                // Step 3: Construct the request body for your new clickwrap version
                ClickwrapRequest clickwrapRequest = BuildUpdateClickwrapVersionRequest();
                var clickwrapId = RequestItemsService.ClickwrapId;

                // Step 4: Call the Click API to create the clickwrap version
                var clickWrap = clickAccountApi.CreateClickwrapVersion(accountId, clickwrapId, clickwrapRequest);

                //Show results
                ViewBag.h1 = "A new clickwrap version was successfully created";
                ViewBag.message = $"The new clickwrap version was created! Clickwrap ID: {clickWrap.ClickwrapId}, Name: {clickWrap.ClickwrapName}.";
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
                Name = "Terms of Service",
                DisplaySettings = new DisplaySettings()
                {
                    DisplayName = "Terms of Service v2",
                    MustRead = true,
                    MustView = false,
                    RequireAccept = false,
                    Downloadable = false,
                    SendToEmail = false,
                    ConsentButtonText = "I Agree",
                    Format = "modal",
                    DocumentDisplay = "document",
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
                Status = "active",
                RequireReacceptance = true
            };

            return clickwrapRequest;
        }
    }
}