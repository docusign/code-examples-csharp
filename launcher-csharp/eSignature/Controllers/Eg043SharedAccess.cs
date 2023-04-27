// <copyright file="Eg040SetDocumentVisibility.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

using System.Linq;
using System.Text.Json;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using ESignature.Examples;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DocuSign.CodeExamples.Views
{

    [Area("eSignature")]
    [Route("Eg043")]
    public class Eg043SharedAccess : EgController
    {
        public Eg043SharedAccess(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService, IConfiguration configuration)
            : base(config, launcherTexts, requestItemsService)
        {
            CodeExampleText = GetExampleText(EgName, ExamplesAPIType.ESignature);
            ViewBag.title = CodeExampleText.ExampleName;
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        public override string EgName => "Eg043";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string agentEmail, string agentName, string activationCode)
        {
            string accessToken = RequestItemsService.User.AccessToken;
            string basePath = RequestItemsService.Session.BasePath + "/restapi";
            string accountId = RequestItemsService.Session.AccountId;

            var impersonatedUserId = _configuration["DocuSignJWT:ImpersonatedUserId"];
            NewUsersSummary user;

            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            try
            {
                user = SharedAccess.ShareAccess(accessToken, basePath, accountId, activationCode, agentEmail, agentName, impersonatedUserId);
                HttpContext.SignOutAsync().GetAwaiter().GetResult();
            }
            catch (ApiException apiException)
            {
                if (apiException.Message.Contains(CodeExampleText.CustomErrorTexts[0].ErrorMessageCheck))
                {
                    ViewBag.fixingInstructions = CodeExampleText.CustomErrorTexts[0].ErrorMessage;
                }

                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;
                ViewBag.SupportingTexts = LauncherTexts.ManifestStructure.SupportingTexts;

                return View("Error");
            }

            ViewBag.h1 = CodeExampleText.ExampleName;
            ViewBag.message = string.Format(CodeExampleText.ResultsPageText, user);
            ViewBag.Locals.Json = JsonSerializer.Serialize(user);

            ViewBag.AdditionalLinkText = CodeExampleText.AdditionalPages[0].Name;
            ViewBag.ConfirmAdditionalLink = "Eg043/AuthRequest";
            ViewBag.OnlyConfirmAdditionalLink = true;

            return View("example_done");
        }

        [SetViewBag]
        [HttpGet]
        [Route("AuthRequest")]
        public ActionResult AuthRequest()
        {
            try
            {
                // Show results
                ViewBag.h1 = CodeExampleText.ExampleName;
                ViewBag.message = CodeExampleText.AdditionalPages[0].ResultsPageText;

                ViewBag.AdditionalLinkText = CodeExampleText.AdditionalPages[0].Name;
                ViewBag.ConfirmAdditionalLink = "/ds/mustAuthenticate";
                ViewBag.OnlyConfirmAdditionalLink = true;
                RequestItemsService.EgName = "Eg043/EnvelopesListStatus";

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;
                ViewBag.SupportingTexts = LauncherTexts.ManifestStructure.SupportingTexts;

                return View("Error");
            }
        }

        [SetViewBag]
        [HttpGet]
        [Route("EnvelopesListStatus")]
        public ActionResult EnvelopesListStatus()
        {
            try
            {
                string accessToken = RequestItemsService.User.AccessToken;
                string basePath = RequestItemsService.Session.BasePath + "/restapi";
                string accountId = RequestItemsService.Session.AccountId;
                var envelopesListStatus = SharedAccess.GetEnvelopesListStatus(accessToken, basePath, accountId);

                // Show results
                if (envelopesListStatus.Envelopes.Any())
                {
                    ViewBag.h1 = CodeExampleText.ExampleName;
                    ViewBag.message = CodeExampleText.AdditionalPages[1].ResultsPageText;
                    ViewBag.Locals.Json = JsonSerializer.Serialize(envelopesListStatus);
                }
                else
                {
                    ViewBag.h1 = CodeExampleText.ExampleName;
                    ViewBag.message = CodeExampleText.AdditionalPages[2].ResultsPageText;
                }

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;
                ViewBag.SupportingTexts = LauncherTexts.ManifestStructure.SupportingTexts;

                return View("Error");
            }
        }
    }
}