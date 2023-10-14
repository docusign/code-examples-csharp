// <copyright file="ShowEmbeddedConsole.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("eg012")]
    public class ShowEmbeddedConsole : EgController
    {
        public ShowEmbeddedConsole(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg012";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string startingView)
        {
            // Data for this method
            // startingView
            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accountId = this.RequestItemsService.Session.AccountId;
            string dsReturnUrl = this.Config.AppUrl + "/dsReturn";
            string envelopeId = this.RequestItemsService.EnvelopeId;

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

            // Call the Examples API method to generate the URL for the embedded console for the specified envelope
            string redirectUrl = global::ESignature.Examples.ShowEmbeddedConsole.CreateEmbeddedConsoleView(
                accessToken,
                basePath,
                accountId,
                startingView,
                dsReturnUrl,
                envelopeId);

            Console.WriteLine(string.Format(this.CodeExampleText.ResultsPageText, redirectUrl));

            return this.Redirect(redirectUrl);
        }
    }
}