// <copyright file="FocusedView.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Views
{
    using System;
    using System.Runtime.InteropServices;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Area("eSignature")]
    [Route("eg044")]
    public class FocusedView : EgController
    {
        private readonly string dsPingUrl;
        private readonly string signerClientId = "1000";
        private readonly string dsReturnUrl;
        private readonly string dsIntegrationKey;
        private readonly IConfiguration configuration;

        public FocusedView(DsConfiguration config, IConfiguration configuration, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.dsPingUrl = config.AppUrl + "/";
            this.dsReturnUrl = config.AppUrl + "/dsReturn";
            this.configuration = configuration;

            this.CodeExampleText = this.GetExampleText("eg044", ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg044";

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
            string docPDF;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                docPDF = Convert.ToBoolean(this.Config.QuickAcg) ? @"../launcher-csharp/" + this.Config.DocPdf : this.Config.DocPdf;
            }
            else
            {
                docPDF = Convert.ToBoolean(this.Config.QuickAcg) ? @"..\\launcher-csharp\\" + this.Config.DocPdf : this.Config.DocPdf;
            }

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

            // Call the method from Examples API to send envelope and generate url for focused view signing
            var result = global::ESignature.Examples.FocusedView.SendEnvelopeWithFocusedView(
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
            this.ViewBag.Url = result.Item2;
            this.ViewBag.IntegrationKey = this.configuration["DocuSign:ClientId"];

            // Redirect the user to the Signing Ceremony
            return this.View("embed");
        }
    }
}