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
        public const string TemplateName = "Web Form Example Template";

        private IConfiguration configuration;

        public CreateAndEmbedForm(
            DsConfiguration config,
            IConfiguration configuration,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.configuration = configuration;
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.WebForms);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "web001";

        [SetViewBag]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckTemplates()
        {
            string basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            string accessToken = this.RequestItemsService.User.AccessToken;
            string accountId = this.RequestItemsService.Session.AccountId;

            //ds-snippet-start:WebForms1Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:WebForms1Step2

            List<EnvelopeTemplate> templates = CreateAndEmbedFormService.GetTemplatesByName(
                docuSignClient,
                accountId,
                TemplateName);

            if (templates == null || templates.Count == 0)
            {
                TemplateSummary template = CreateAndEmbedFormService.CreateTemplate(
                    docuSignClient,
                    accountId,
                    this.Config.DocumentTemplatePdf,
                    TemplateName);

                this.RequestItemsService.WebFormsTemplateId = template.TemplateId;
            }
            else
            {
                this.RequestItemsService.WebFormsTemplateId = templates.First().TemplateId;
            }

            CreateAndEmbedFormService.AddTemplateIdToForm(
                this.Config.WebFormConfig,
                this.RequestItemsService.WebFormsTemplateId);

            this.ViewBag.CodeExampleText = this.CodeExampleText;
            this.ViewBag.Description = this.CodeExampleText.AdditionalPages
                .First(x => x.Name == "create_web_form").ResultsPageText;

            return this.View("embedForm");
        }

        [MustAuthenticate]
        [SetViewBag]
        [Route("EmbedForm")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmbedForm()
        {
            string basePath = this.RequestItemsService.Session.WebFormsBasePath;
            string accessToken = this.RequestItemsService.User.AccessToken;
            string accountId = this.RequestItemsService.Session.AccountId;

            var docuSignClient = new DocuSign.WebForms.Client.DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            WebFormSummaryList forms = CreateAndEmbedFormService.GetForms(
                docuSignClient,
                accountId);

            if (forms.Items == null || forms.Items.Count == 0)
            {
                this.ViewBag.CodeExampleText = this.CodeExampleText;
                this.ViewBag.Description = this.CodeExampleText.AdditionalPages
                    .First(x => x.Name == "create_web_form").ResultsPageText;

                return this.View("embedForm");
            }

            string formId = forms.Items.First(x => x.FormProperties.Name == TemplateName).Id;
            WebFormInstance form = CreateAndEmbedFormService.CreateInstance(
                docuSignClient,
                accountId,
                formId);

            this.ViewBag.InstanceToken = form.InstanceToken;
            this.ViewBag.Url = form.FormUrl;
            this.ViewBag.IntegrationKey = this.configuration["DocuSign:ClientId"];

            return this.View("Embed");
        }
    }
}
