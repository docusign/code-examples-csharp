// <copyright file="Eg040SetDocumentVisibility.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Views
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("Eg040")]
    public class Eg040SetDocumentVisibility : EgController
    {
        public Eg040SetDocumentVisibility(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg040";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(
            string signer1Email,
            string signer1Name,
            string signer2Email,
            string signer2Name,
            string ccEmail,
            string ccName)
        {
            string accessToken = this.RequestItemsService.User.AccessToken;
            string basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            string accountId = this.RequestItemsService.Session.AccountId;
            string envelopeId = string.Empty;

            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            try
            {
                envelopeId = global::ESignature.Examples.SetDocumentVisibility.SendEnvelopeWithEnvelopeVisibility(
                    signer1Email,
                    signer1Name,
                    signer2Email,
                    signer2Name,
                    ccEmail,
                    ccName,
                    accessToken,
                    basePath,
                    accountId,
                    this.Config.DocPdf,
                    this.Config.DocDocx,
                    this.Config.DocHtml);
            }
            catch (ApiException apiException)
            {
                if (apiException.Message.Contains(this.CodeExampleText.CustomErrorTexts[0].ErrorMessageCheck))
                {
                    this.ViewBag.fixingInstructions = this.CodeExampleText.CustomErrorTexts[0].ErrorMessage;
                }

                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, envelopeId);

            return this.View("example_done");
        }
    }
}