// <copyright file="Eg001SetConnectedFieldsController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System.Linq;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Examples;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

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

            object extensionApps = SetConnectedFields.GetConnectedFieldsTabGroupsAsync(accountId, accessToken).Result;
            object filteredExtensionApps = SetConnectedFields.FilterData((JArray)extensionApps);
            var extensionAppsString = JsonConvert.SerializeObject(filteredExtensionApps);

            this.ViewBag.ExtensionApps = extensionAppsString;
            this.RequestItemsService.ExtensionApps = extensionAppsString;

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

            JArray extensionApp = JArray.Parse(this.RequestItemsService.ExtensionApps);
            JObject selectedApp = extensionApp.FirstOrDefault(app => (string)app["appId"] == appId) as JObject;

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
