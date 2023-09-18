// <copyright file="SigningViaEmail.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("eg002")]
    public class SigningViaEmail : EgController
    {
        public SigningViaEmail(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg002";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Check the token with minimal buffer time.
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

            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accountId = this.RequestItemsService.Session.AccountId;

            // Call the Examples API method to create and send an envelope via email
            var envelopeId = global::ESignature.Examples.SigningViaEmail.SendEnvelopeViaEmail(
                signerEmail,
                signerName,
                ccEmail,
                ccName,
                accessToken,
                basePath,
                accountId,
                this.Config.DocDocx,
                this.Config.DocPdf,
                this.RequestItemsService.Status);

            this.RequestItemsService.EnvelopeId = envelopeId;

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, envelopeId);
            return this.View("example_done");
        }
    }
}