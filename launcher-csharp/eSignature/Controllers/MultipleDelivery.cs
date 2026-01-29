// <copyright file="MultipleDelivery.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("Eg046")]
    public class MultipleDelivery : EgController
    {
        public MultipleDelivery(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg046";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerName, string signerCountryCode, string signerPhoneNumber, string signerEmail, string ccName, string ccEmail, string ccCountryCode, string ccPhoneNumber, string deliveryMethod)
        {
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accessToken = this.RequestItemsService.User.AccessToken;
            var accountId = this.RequestItemsService.Session.AccountId;

            try
            {
                EnvelopeDefinition envelopeDefinition = global::ESignature.Examples.MultipleDelivery.MakeEnvelope(
                    signerName,
                    signerEmail,
                    signerCountryCode,
                    signerPhoneNumber,
                    ccName,
                    ccEmail,
                    ccCountryCode,
                    ccPhoneNumber,
                    this.Config.DocDocx,
                    this.Config.DocPdf,
                    this.RequestItemsService.Status,
                    deliveryMethod);

                string envelopeId = global::ESignature.Examples.MultipleDelivery.SendRequestByMultipleChannels(
                    accessToken,
                    basePath,
                    accountId,
                    envelopeDefinition);

                this.RequestItemsService.EnvelopeId = envelopeId;

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, envelopeId);
                return this.View("example_done");
            }
            catch (ApiException apiException)
            {
                if (apiException.Message.Contains(this.CodeExampleText.CustomErrorTexts[0].ErrorMessageCheck))
                {
                    this.ViewBag.fixingInstructions = this.CodeExampleText.CustomErrorTexts[0].ErrorMessage;
                }
                else
                {
                    this.ViewBag.errorMessage = apiException.Message;
                }

                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }
        }
    }
}