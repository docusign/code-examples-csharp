// <copyright file="ApplyBrandToTemplate.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("Eg030")]
    public class ApplyBrandToTemplate : EgController
    {
        public ApplyBrandToTemplate(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, Common.ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg030";

        [Common.SetViewBag]
        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string cCName, string brandId)
        {
            // Check the token with minimal buffer time.
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            // Data for this method
            // signerEmail
            // signerName
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";

            // Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Call the eSignature
            var results = global::ESignature.Examples.ApplyBrandToTemplate.CreateEnvelopeFromTemplateWithBrand(
                signerEmail,
                signerName,
                ccEmail,
                cCName,
                brandId,
                this.RequestItemsService.TemplateId,
                accessToken,
                basePath,
                accountId,
                this.RequestItemsService.Status);

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, results.EnvelopeId);
            return this.View("example_done");
        }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();

            // Data for this method
            // signerEmail
            // signerName
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var accountsApi = new AccountsApi(docuSignClient);
            var brands = accountsApi.ListBrands(accountId);
            this.ViewBag.Brands = brands.Brands;
            var templatesApi = new TemplatesApi(docuSignClient);
            var templates = templatesApi.ListTemplates(accountId);
            this.ViewBag.EnvelopeTemplates = templates.EnvelopeTemplates;
        }
    }
}