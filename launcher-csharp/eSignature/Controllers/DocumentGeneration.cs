// <copyright file="DocumentGeneration.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

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
    [Route("Eg042")]
    public class DocumentGeneration : EgController
    {
        public DocumentGeneration(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgName, ExamplesAPIType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg042";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string candidateEmail, string candidateName, string managerName, string jobTitle,
            string salary, DateTime startDate)
        {
            string accessToken = RequestItemsService.User.AccessToken;
            string basePath = RequestItemsService.Session.BasePath + "/restapi";
            string accountId = RequestItemsService.Session.AccountId;
            string envelopeId = string.Empty;

            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            try
            {
                envelopeId = global::ESignature.Examples.DocumentGeneration.DocumentGenerationExample(
                    accessToken,
                    basePath,
                    accountId,
                    candidateEmail,
                    candidateName,
                    managerName,
                    jobTitle,
                    salary,
                    startDate,
                    Config.OfferDocDocx);
            }
            catch (ApiException apiException)
            {

                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return View("Error");
            }

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, envelopeId);

            return View("example_done");
        }
    }
}