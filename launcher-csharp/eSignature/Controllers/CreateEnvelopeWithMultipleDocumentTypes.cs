// <copyright file="CreateEnvelopeWithMultipleDocumentTypes.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using System.Net;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("eg010")]
    public class CreateEnvelopeWithMultipleDocumentTypes : EgController
    {
        public CreateEnvelopeWithMultipleDocumentTypes(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg010";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Data for this method
            // signerEmail
            // signerName
            // ccEmail
            // ccName
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accountId = this.RequestItemsService.Session.AccountId;

            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation
                // so it could be restarted automatically.
                // But since it should be rare to have a token issue here,
                // we'll make the user re-enter the form data after
                // authentication.
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            // Call the Examples API method to create the envelope multiple types of documents and send it
            (bool statusOk, string envelopeId, string errorCode, string errorMessage, _) =
                global::ESignature.Examples.CreateEnvelopeWithMultipleDocumentTypes.CreateAndSendEnvelope(
                    signerEmail,
                    signerName,
                    ccEmail,
                    ccName,
                    this.Config.DocDocx,
                    this.Config.DocPdf,
                    accessToken,
                    basePath,
                    accountId);

            // Process results
            if (statusOk)
            {
                this.RequestItemsService.EnvelopeId = envelopeId;
                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, envelopeId);
                return this.View("example_done");
            }
            else
            {
                this.ViewBag.errorCode = errorCode;
                this.ViewBag.errorMessage = errorMessage;
                return this.View("error");
            }
        }
    }
}