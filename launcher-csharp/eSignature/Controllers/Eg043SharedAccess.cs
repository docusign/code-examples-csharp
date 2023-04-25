// <copyright file="Eg040SetDocumentVisibility.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

using DocuSign.eSign.Model;
using ESignature.Examples;
using Microsoft.AspNetCore.Authentication;

namespace DocuSign.CodeExamples.Views
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using eSignature.Examples;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("Eg043")]
    public class Eg043SharedAccess : EgController
    {
        public Eg043SharedAccess(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgName, ExamplesAPIType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg043";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string agentEmail, string agentName, string activationCode)
        {
            string accessToken = RequestItemsService.User.AccessToken;
            string basePath = RequestItemsService.Session.BasePath + "/restapi";
            string accountId = RequestItemsService.Session.AccountId;
            NewUsersSummary user;

            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                RequestItemsService.EgName = EgName;
                AuthenticationHttpContextExtensions.SignOutAsync(HttpContext).GetAwaiter().GetResult();
                return Redirect("/ds/mustAuthenticate");
            }

            try
            {
                user = SharedAccess.ShareAccess(accessToken, basePath, accountId, activationCode, agentEmail, agentName);

                AuthenticationHttpContextExtensions.SignOutAsync(HttpContext).GetAwaiter().GetResult();
            }
            catch (ApiException apiException)
            {
                if (apiException.Message.Contains(this.CodeExampleText.CustomErrorTexts[0].ErrorMessageCheck))
                {
                    ViewBag.fixingInstructions = this.CodeExampleText.CustomErrorTexts[0].ErrorMessage;
                }

                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return View("Error");
            }

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, user);

            return View("example_done");
        }
    }
}