// <copyright file="CFR11EmbeddedSigning.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("eg041")]
    public class Cfr11EmbeddedSigning : EgController
    {
        public Cfr11EmbeddedSigning(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg041";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName, string signerCountryCode, string signerPhoneNumber)
        {
            // Data for this method
            // signerEmail
            // signerName
            // ccEmail
            // ccName
            // startingView
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accountId = this.RequestItemsService.Session.AccountId;
            string dsReturnUrl = this.Config.AppUrl + "/dsReturn";

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

            // Call the Examples API method to create the envelope and send it using embedded sending
            var redirectUrl = global::ESignature.Examples.CfrPart11EmbeddedSending.EmbeddedSigning(
                signerEmail,
                signerName,
                accessToken,
                basePath,
                accountId,
                signerCountryCode,
                signerPhoneNumber,
                this.Config.DocPdf,
                this.Config.AppUrl + "/dsReturn");

            Console.WriteLine(string.Format(this.CodeExampleText.ResultsPageText, redirectUrl));

            return this.Redirect(redirectUrl);
        }
    }
}