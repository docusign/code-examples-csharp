// <copyright file="Eg001EmbeddedSigningController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Views
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using global::ESignature.Examples;
    using Microsoft.AspNetCore.Mvc;

    [Route("eg001")]
    public class Eg001EmbeddedSigningController : EgController
    {
        private readonly string dsPingUrl;
        private readonly string signerClientId = "1000";
        private readonly string dsReturnUrl;

        public Eg001EmbeddedSigningController(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.dsPingUrl = config.AppUrl + "/";
            this.dsReturnUrl = config.AppUrl + "/dsReturn";

            this.CodeExampleText = this.GetExampleText("eg001", ExamplesAPIType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => Convert.ToBoolean(this.Config.QuickACG) ? "quickEmbeddedSigning" : "eg001";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName)
        {
            // Data for this method
            // signerEmail
            // signerName
            // dsPingUrl -- class global
            // signerClientId -- class global
            // dsReturnUrl -- class global
            string accessToken = this.RequestItemsService.User.AccessToken;
            string basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            string accountId = this.RequestItemsService.Session.AccountId;
            string docPDF = Convert.ToBoolean(this.Config.QuickACG) ? @"..\\launcher-csharp\\" + this.Config.DocPdf : this.Config.DocPdf;

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

            // Call the method from Examples API to send envelope and generate url for embedded signing
            var result = EmbeddedSigningCeremony.SendEnvelopeForEmbeddedSigning(
                signerEmail,
                signerName,
                this.signerClientId,
                accessToken,
                basePath,
                accountId,
                docPDF,
                this.dsReturnUrl,
                this.dsPingUrl);

            // Save for future use within the example launcher
            this.RequestItemsService.EnvelopeId = result.Item1;

            // Redirect the user to the Signing Ceremony
            return this.Redirect(result.Item2);
        }
    }
}