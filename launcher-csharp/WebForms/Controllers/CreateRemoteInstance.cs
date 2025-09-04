// <copyright file="CreateRemoteInstance.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.WebForms.Controllers
{
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

    [Area("WebForms")]
    [Route("web002")]
    public class CreateRemoteInstance : EgController
    {
        public const string TemplateName = "Web Form Example Template";

        public CreateRemoteInstance(
            DsConfiguration config,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.WebForms);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "web002";

        [SetViewBag]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckAndCreateTemplates()
        {
            string basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            string accessToken = this.RequestItemsService.User.AccessToken;
            string accountId = this.RequestItemsService.Session.AccountId;

            try
            {
                List<EnvelopeTemplate> templates = CreateRemoteInstanceService.GetTemplatesByName(
                    basePath,
                    accessToken,
                    accountId,
                    TemplateName);

                string templateId;

                if (templates == null || templates.Count == 0)
                {
                    TemplateSummary template = CreateRemoteInstanceService.CreateTemplate(
                        basePath,
                        accessToken,
                        accountId,
                        this.Config.DocumentTemplatePdf,
                        TemplateName);

                    templateId = template.TemplateId;
                }
                else
                {
                    templateId = templates.First().TemplateId;
                }

                this.RequestItemsService.WebFormsTemplateId = templateId;

                CreateRemoteInstanceService.AddTemplateIdToForm(
                    this.Config.WebFormConfig,
                    templateId);

                this.ViewBag.CodeExampleText = this.CodeExampleText;
                this.ViewBag.Description = this.CodeExampleText.AdditionalPages
                    .First(x => x.Name == "create_web_form").ResultsPageText;

                return this.View("createWebForm");
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }
        }

        [MustAuthenticate]
        [SetViewBag]
        [Route("CreateInstance")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateInstance()
        {
            string basePath = this.RequestItemsService.Session.WebFormsBasePath;
            string accessToken = this.RequestItemsService.User.AccessToken;
            string accountId = this.RequestItemsService.Session.AccountId;

            try
            {
                WebFormSummaryList forms = CreateRemoteInstanceService.GetFormsByName(
                    basePath,
                    accessToken,
                    accountId,
                    TemplateName);

                if (forms.Items == null || forms.Items.Count == 0)
                {
                    this.ViewBag.message = this.CodeExampleText.CustomErrorTexts[0].ErrorMessage;
                }
                else
                {
                    var formItem = forms.Items.First(x => x.FormProperties.Name == TemplateName);
                    WebFormInstance form = CreateRemoteInstanceService.CreateInstance(
                        basePath,
                        accessToken,
                        accountId,
                        formItem.Id,
                        this.Config.SignerEmail,
                        this.Config.SignerName);

                    this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, form.Envelopes[0].Id, form.Id);
                }

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;

                return this.View("example_done");
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }
        }
    }
}
