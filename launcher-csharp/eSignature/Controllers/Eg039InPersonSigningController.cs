// <copyright file="Eg039InPersonSigningController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Views
{
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using global::ESignature.Examples;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("Eg039")]
    public class Eg039InPersonSigningController : EgController
    {
        public Eg039InPersonSigningController(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.DsPingUrl = config.AppUrl + "/";
            this.DsReturnUrl = config.AppUrl + "/dsReturn";
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg039";

        private string DsPingUrl { get; set; }

        private string DsReturnUrl { get; set; }

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerName)
        {
            string accessToken = this.RequestItemsService.User.AccessToken;
            string basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            string accountId = this.RequestItemsService.Session.AccountId;
            string hostEmail = this.RequestItemsService.AuthenticatedUserEmail;
            string hostName = this.RequestItemsService.User.Name;
            string redirectUrl = string.Empty;

            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            try
            {
                redirectUrl = InPersonSigning.SendEnvelopeForInPersonSigning(
                    hostEmail,
                    hostName,
                    signerName,
                    accessToken,
                    basePath,
                    accountId,
                    this.Config.DocPdf,
                    this.DsReturnUrl,
                    this.DsPingUrl);
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