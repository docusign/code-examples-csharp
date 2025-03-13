// <copyright file="Eg001SetConnectedFieldsController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using System.Linq;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.ConnectedFields.Models;
    using DocuSign.CodeExamples.Examples;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    [Area("ConnectedFields")]
    [Route("cf001")]
    public class Eg001SetConnectedFieldsController : EgController
    {
        public Eg001SetConnectedFieldsController(DsConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService)
            : base(dsConfig, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ConnectedFields);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "cf001";

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            IActionResult actionResult = base.Get();
            if (this.RequestItemsService.EgName == this.EgName)
            {
                return actionResult;
            }

            Guid? organizationId = this.RequestItemsService.OrganizationId;
            string accessToken = this.RequestItemsService.User.AccessToken;

            ExtensionApps extensionApps = SetConnectedFields.GetConnectedFieldsAsync(accessToken, this.RequestItemsService.User.AccountId).Result;

            var filteredExtensionApps = extensionApps.Applications.FindAll(extension => !extension.Tabs.Where(tabs => tabs.ExtensionData.ActionContract.Contains("Verify") == true).IsNullOrEmpty());

            this.ViewBag.ExtensionApps = filteredExtensionApps;
            return this.View(this.EgName, this);
        }

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName, string extensionName)
        {
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            var accessToken = this.RequestItemsService.User.AccessToken;
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accountId = this.RequestItemsService.Session.AccountId;

            var envelopeId = SetConnectedFields.SendEnvelopeViaEmail(
                signerEmail,
                signerName,
                null, // pass extension here
                accessToken,
                basePath,
                accountId,
                this.Config.DocPdf);

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, envelopeId);
            return this.View("example_done");
        }
    }
}
