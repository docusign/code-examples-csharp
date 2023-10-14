// <copyright file="EmbeddedSending.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("eg011")]
    public class EmbeddedSending : EgController
    {
        public EmbeddedSending(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg011";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName, string startingView)
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
            var redirectUrl = global::ESignature.Examples.EmbeddedSending.SendEnvelopeUsingEmbeddedSending(
                signerEmail,
                signerName,
                ccEmail,
                ccName,
                this.Config.DocDocx,
                this.Config.DocPdf,
                accessToken,
                basePath,
                accountId,
                startingView,
                dsReturnUrl);

            Console.WriteLine(string.Format(this.CodeExampleText.ResultsPageText, redirectUrl));
            return this.Redirect(redirectUrl);
        }
    }
}