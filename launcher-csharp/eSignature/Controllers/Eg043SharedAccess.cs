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
using Org.BouncyCastle.Ocsp;

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
            _accessToken = RequestItemsService.User.AccessToken;
            _basePath = RequestItemsService.Session.BasePath + "/restapi";
            _accountId = RequestItemsService.Session.AccountId;
        }

        private readonly IConfiguration _configuration;
        private string _agentUserId;
        private string _accessToken;
        private string _basePath;
        private string _accountId;

        public override string EgName => "Eg043";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string agentEmail, string agentName, string activationCode)
        {
            try
            {
                NewUsersSummary user;

                bool tokenOk = CheckToken(3);
                if (!tokenOk)
                {
                    RequestItemsService.EgName = EgName;
                    return Redirect("/ds/mustAuthenticate");
                }

                user = SharedAccess.ShareAccess(_accessToken, _basePath, _accountId, activationCode, agentEmail, agentName);
                var agentUserId = user.NewUsers.FirstOrDefault()?.UserId;

                ViewBag.h1 = CodeExampleText.ExampleName;
                ViewBag.message = string.Format(CodeExampleText.ResultsPageText, user);
                ViewBag.Locals.Json = JsonSerializer.Serialize(user);

                ViewBag.AdditionalLinkText = CodeExampleText.AdditionalPages[0].Name;
                ViewBag.ConfirmAdditionalLink = $"Eg043/AuthRequest/{agentUserId}";
                ViewBag.OnlyConfirmAdditionalLink = true;

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
        [Route("AuthRequest/{agentId}")]
        public ActionResult AuthRequest(string agentId)
        {
            try
            {
                var impersonatedUserId = _configuration["DocuSignJWT:ImpersonatedUserId"];

                SharedAccess.CreateUserAuthorization(_accessToken, _basePath, _accountId, impersonatedUserId, agentId);
                HttpContext.SignOutAsync().GetAwaiter().GetResult();

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
                var envelopesListStatus = SharedAccess.GetEnvelopesListStatus(_accessToken, _basePath, _accountId);

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