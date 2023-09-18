// <copyright file="CreateNewTemplate.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("eg008")]
    public class CreateNewTemplate : EgController
    {
        public CreateNewTemplate(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg008";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create()
        {
            // Data for this method
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

            // Call the Examples API method to create a new DocuSign template
            (bool createdNewTemplate, string templateId, string resultsTemplateName) = global::ESignature.Examples.CreateNewTemplate.CreateTemplate(
                 accessToken, basePath, accountId, this.Config.DocPdf);

            // Save the templateId
            this.RequestItemsService.TemplateId = templateId;
            string msg = createdNewTemplate ?
                    "The template has been created!" :
                    "The template already exists in your account.";
            this.ViewBag.message = msg + string.Format(this.CodeExampleText.ResultsPageText, resultsTemplateName, templateId);

            return this.View("example_done");
        }
    }
}