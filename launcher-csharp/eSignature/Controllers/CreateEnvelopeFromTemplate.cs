// <copyright file="CreateEnvelopeFromTemplate.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("eg009")]
    public class CreateEnvelopeFromTemplate : EgController
    {
        public CreateEnvelopeFromTemplate(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg009";

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
            var templateId = this.RequestItemsService.TemplateId;

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

            // Call the Examples API method to create the envelope from template and send it
            string envelopeId = global::ESignature.Examples.CreateEnvelopeFromTemplate.SendEnvelopeFromTemplate(
                signerEmail,
                signerName,
                ccEmail,
                ccName,
                accessToken,
                basePath,
                accountId,
                templateId);

            // Process results
            this.RequestItemsService.EnvelopeId = envelopeId;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, envelopeId);
            return this.View("example_done");
        }
    }
}