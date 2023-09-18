// <copyright file="ResponsiveSigning.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Views
{
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("Eg038")]
    public class ResponsiveSigning : EgController
    {
        private string dsPingUrl;
        private string signerClientId = "1000";
        private string dsReturnUrl;

        public ResponsiveSigning(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.dsPingUrl = config.AppUrl + "/";
            this.dsReturnUrl = config.AppUrl + "/dsReturn";
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg038";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            string accessToken = this.RequestItemsService.User.AccessToken;
            string basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            string accountId = this.RequestItemsService.Session.AccountId;
            string redirectUrl = string.Empty;

            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            try
            {
                redirectUrl = global::ESignature.Examples.ResponsiveSigning.CreateEnvelopeFromHtml(
                    signerEmail,
                    signerName,
                    ccEmail,
                    ccName,
                    this.signerClientId,
                    accessToken,
                    basePath,
                    accountId,
                    this.dsReturnUrl,
                    this.dsPingUrl);
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }

            return this.Redirect(redirectUrl);
        }
    }
}