// <copyright file="CreateAndEmbedForm.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.WebForms.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using DocuSign.WebForms.Examples;
    using DocuSign.WebForms.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Area("WebForms")]
    [Route("web001")]
    public class CreateAndEmbedForm : EgController
    {
        private IConfiguration configuration;

        public CreateAndEmbedForm(
            DsConfiguration config,
            IConfiguration configuration,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.configuration = configuration;
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Connect);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "web001";

        [SetViewBag]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckTheTemplates()
        {
            string basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            string accessToken = this.RequestItemsService.User.AccessToken;
            string accountId = this.RequestItemsService.Session.AccountId;

            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            List<EnvelopeTemplate> templates = CreateAndEmbedFormService.CheckIfTemplateExists(docuSignClient, accountId);
            if (templates == null || templates.Count == 0)
            {
                var template = CreateAndEmbedFormService.CreateTemplate(
                    docuSignClient,
                    accountId,
                    this.Config.DocumentTemplatePdf);

                this.RequestItemsService.WebFormsTemplateId = template.TemplateId;
            }
            else
            {
                this.RequestItemsService.WebFormsTemplateId = templates.First().TemplateId;
            }

            CreateAndEmbedFormService.AddTemplateIdToForm(
                this.Config.WebFormConfig,
                this.RequestItemsService.WebFormsTemplateId);

            return this.View("embedForm");
        }

        [MustAuthenticate]
        [SetViewBag]
        [Route("EmbedForm")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmbedForm(string formId)
        {
            string basePath = this.RequestItemsService.Session.WebFormsBasePath;
            string accessToken = this.RequestItemsService.User.AccessToken;
            string accountId = this.RequestItemsService.Session.AccountId;

            var docuSignClient = new DocuSign.WebForms.Client.DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            WebFormInstance form = CreateAndEmbedFormService.CreateInstance(
                docuSignClient,
                Guid.Parse(accountId),
                formId);

            this.ViewBag.InstanceToken = form.InstanceToken;
            this.ViewBag.Url = form.FormUrl;
            this.ViewBag.IntegrationKey = this.configuration["DocuSign:ClientId"];

            return this.View("Embed");
        }
    }
}
