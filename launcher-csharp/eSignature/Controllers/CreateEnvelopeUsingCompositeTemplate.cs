// <copyright file="CreateEnvelopeUsingCompositeTemplate.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("eg013")]
    public class CreateEnvelopeUsingCompositeTemplate : EgController
    {
        private string signerClientId = "1000";

        public CreateEnvelopeUsingCompositeTemplate(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "eg013";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName, string item, string quantity)
        {
            // Data for this method
            // signerEmail
            // signerName
            // ccEmail
            // ccName
            // item
            // quantity
            // signerClientId -- class global
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

            // Call the Examples API method to create an envelope using both a template and a document
            string redirectUrl = global::ESignature.Examples.CreateEnvelopeUsingCompositeTemplate.CreateEnvelopeFromCompositeTemplate(
                signerEmail,
                signerName,
                ccEmail,
                ccName,
                accessToken,
                basePath,
                accountId,
                item,
                quantity,
                dsReturnUrl,
                this.signerClientId,
                this.RequestItemsService.TemplateId);

            return this.Redirect(redirectUrl);
        }
    }
}