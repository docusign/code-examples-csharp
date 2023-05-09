// <copyright file="Eg040SetDocumentVisibility.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

using System.Linq;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using ESignature.Examples;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using JsonSerializer=System.Text.Json.JsonSerializer;

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
            _accessToken = RequestItemsService?.User?.AccessToken;
            _basePath = RequestItemsService?.Session?.BasePath + "/restapi";
            _accountId = RequestItemsService?.Session?.AccountId;
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
                string agentUserId;
                bool tokenOk = this.CheckToken(3);
                if (!tokenOk)
                {
                    this.RequestItemsService.EgName = this.EgName;
                    return this.Redirect("/ds/mustAuthenticate");
                }

                UserInformation userInformation = SharedAccess.GetUserInfo(this._accessToken, this._basePath, this._accountId, agentEmail);

                if (userInformation == null)
                {
                    NewUsersSummary user = SharedAccess.ShareAccess(this._accessToken, this._basePath, this._accountId, activationCode, agentEmail, agentName);
                    agentUserId = user.NewUsers.FirstOrDefault()?.UserId;

                    this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, user);
                    this.ViewBag.Locals.Json = JsonConvert.SerializeObject(user, Formatting.Indented);
                }
                else
                {
                    agentUserId = userInformation.UserId;

                    this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, userInformation);
                    this.ViewBag.Locals.Json = JsonConvert.SerializeObject(userInformation, Formatting.Indented);
                }

                this.ViewBag.h1 = "Agent user created";
                this.ViewBag.AdditionalLinkText = this.CodeExampleText.AdditionalPages[0].Name;
                this.ViewBag.ConfirmAdditionalLink = $"Eg043/AuthRequest/{agentUserId}";
                this.ViewBag.OnlyConfirmAdditionalLink = true;

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

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
                var userId = SharedAccess.GetCurrentUserInfo(this._basePath, this._accessToken).Sub;

                SharedAccess.CreateUserAuthorization(this._accessToken, this._basePath, this._accountId, userId, agentId);
                HttpContext.SignOutAsync().GetAwaiter().GetResult();

                this.RequestItemsService.PrincipalUserId = userId;
                // Show results
                this.ViewBag.h1 = "Authenticate as the agent";
                this.ViewBag.message = this.CodeExampleText.AdditionalPages[0].ResultsPageText;

                this.ViewBag.AdditionalLinkText = this.CodeExampleText.AdditionalPages[0].Name;
                this.ViewBag.ConfirmAdditionalLink = "/ds/mustAuthenticate";
                this.ViewBag.OnlyConfirmAdditionalLink = true;
                this.RequestItemsService.EgName = "Eg043/EnvelopesListStatus";

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                this.ViewBag.h1 = "Authenticate as the agent";
                this.ViewBag.message = this.CodeExampleText.AdditionalPages[3].ResultsPageText;
                this.ViewBag.AdditionalLinkText = this.CodeExampleText.AdditionalPages[3].Name;
                this.ViewBag.ConfirmAdditionalLink = $"/Eg043/AuthRequest/{agentId}";
                this.ViewBag.OnlyConfirmAdditionalLink = true;

                return View("example_done");
            }
        }

        [SetViewBag]
        [HttpGet]
        [Route("EnvelopesListStatus")]
        public ActionResult EnvelopesListStatus()
        {
            try
            {
                var envelopesListStatus = SharedAccess.GetEnvelopesListStatus(this._accessToken, this._basePath, this._accountId, this.RequestItemsService.PrincipalUserId);

                // Show results
                if (envelopesListStatus.Envelopes != null && envelopesListStatus.Envelopes.Any())
                {
                    this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                    this.ViewBag.message = this.CodeExampleText.AdditionalPages[1].ResultsPageText;
                    this.ViewBag.Locals.Json = JsonConvert.SerializeObject(envelopesListStatus, Formatting.Indented);
                }
                else
                {
                    this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                    this.ViewBag.message = this.CodeExampleText.AdditionalPages[2].ResultsPageText;
                }

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return View("Error");
            }
        }
    }
}