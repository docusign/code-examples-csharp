// <copyright file="Eg043SharedAccess.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Views
{
    using System.Linq;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using global::ESignature.Examples;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    [Area("eSignature")]
    [Route("Eg043")]
    public class Eg043SharedAccess : EgController
    {
        private readonly string accessToken;
        private readonly string basePath;
        private readonly string accountId;

        public Eg043SharedAccess(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService, IConfiguration configuration)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
            this.accessToken = this.RequestItemsService?.User?.AccessToken;
            this.basePath = this.RequestItemsService?.Session?.BasePath + "/restapi";
            this.accountId = this.RequestItemsService?.Session?.AccountId;
        }

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

                UserInformation userInformation = SharedAccess.GetUserInfo(this.accessToken, this.basePath, this.accountId, agentEmail);

                if (userInformation == null)
                {
                    NewUsersSummary user = SharedAccess.ShareAccess(this.accessToken, this.basePath, this.accountId, activationCode, agentEmail, agentName);
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

                return this.View("example_done");
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }
        }

        [SetViewBag]
        [HttpGet]
        [Route("AuthRequest/{agentId}")]
        public ActionResult AuthRequest(string agentId)
        {
            try
            {
                var userId = SharedAccess.GetCurrentUserInfo(this.basePath, this.accessToken).Sub;

                SharedAccess.CreateUserAuthorization(this.accessToken, this.basePath, this.accountId, userId, agentId);
                this.HttpContext.SignOutAsync().GetAwaiter().GetResult();

                this.Config.PrincipalUserId = userId;
                // Show results
                this.ViewBag.h1 = "Authenticate as the agent";
                this.ViewBag.message = this.CodeExampleText.AdditionalPages[0].ResultsPageText;

                this.ViewBag.AdditionalLinkText = this.CodeExampleText.AdditionalPages[0].Name;
                this.ViewBag.ConfirmAdditionalLink = "/ds/mustAuthenticate";
                this.ViewBag.OnlyConfirmAdditionalLink = true;
                this.RequestItemsService.EgName = "Eg043/EnvelopesListStatus";
                this.Config.IsLoggedInAfterEg043 = true;

                return this.View("example_done");
            }
            catch (ApiException)
            {
                this.ViewBag.h1 = "Authenticate as the agent";
                this.ViewBag.message = this.CodeExampleText.AdditionalPages[3].ResultsPageText;
                this.ViewBag.AdditionalLinkText = this.CodeExampleText.AdditionalPages[3].Name;
                this.ViewBag.ConfirmAdditionalLink = $"/Eg043/AuthRequest/{agentId}";
                this.ViewBag.OnlyConfirmAdditionalLink = true;

                return this.View("example_done");
            }
        }

        [SetViewBag]
        [HttpGet]
        [Route("EnvelopesListStatus")]
        public ActionResult EnvelopesListStatus()
        {
            try
            {
                var envelopesListStatus = SharedAccess.GetEnvelopesListStatus(this.accessToken, this.basePath, this.accountId, this.Config.PrincipalUserId);

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

                return this.View("example_done");
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }
        }
    }
}