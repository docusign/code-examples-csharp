// <copyright file="Eg001SetConnectedFieldsController.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System.Linq;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Examples;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

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

            string accessToken = this.RequestItemsService.User.AccessToken;
            string accountId = this.RequestItemsService.Session.AccountId;
            var basePath = this.RequestItemsService.Session.IamBasePath;
            var connectedFields = SetConnectedFields.GetConnectedFieldsTabGroupsAsync(
                basePath,
                accountId,
                accessToken).Result;
            var filteredConnectedFields = SetConnectedFields.FilterData(connectedFields);

            this.ViewBag.ExtensionApps = filteredConnectedFields;
            this.RequestItemsService.ExtensionApps = filteredConnectedFields;

            return this.View(this.EgName, this);
        }

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerEmail, string signerName, string appId)
        {
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            var extensionApp = this.RequestItemsService.ExtensionApps;
            var selectedApp = extensionApp.FirstOrDefault(tab => tab.AppId == appId);

            string accessToken = this.RequestItemsService.User.AccessToken;
            string basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            string accountId = this.RequestItemsService.Session.AccountId;

            string envelopeId = SetConnectedFields.SendEnvelopeViaEmail(
                basePath,
                accessToken,
                accountId,
                signerEmail,
                signerName,
                this.Config.DocPdf,
                selectedApp);

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, envelopeId);

            return this.View("example_done");
        }
    }
}
