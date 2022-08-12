﻿// <copyright file="Eg001GetMonitoringDataController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using DocuSign.CodeExamples.Monitor.Examples;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Monitor")]
    [Route("monitorExample001")]
    public class Eg001GetMonitoringDataController : EgController
    {
        private readonly GetMonitoringDataFunc getMonitoringDataFunc = new GetMonitoringDataFunc();

        private readonly IRequestItemsService requestItemsService;

        public Eg001GetMonitoringDataController(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.requestItemsService = requestItemsService;

            this.CodeExampleText = this.GetExampleText(EgNumber);
            this.ViewBag.title = this.CodeExampleText.PageTitle;
        }

        public override int EgNumber => 1;

        public override string EgName => "monitorExample001";

        [MustAuthenticate]
        [HttpPost]
        public IActionResult Create()
        {
            // Obtain your JWT authentication token
            this.requestItemsService.UpdateUserFromJWT();

            // Preparing data for this method
            string accessToken = this.RequestItemsService.User.AccessToken;
            string requestPath = "https://lens-d.docusign.net/api/v2.0/datasets/monitor/";

            // Getting monitoring data
            var results = this.getMonitoringDataFunc.Invoke(accessToken, requestPath);

            // Process results
            this.ViewBag.h1 = this.CodeExampleText.ResultsPageHeader;
            this.ViewBag.message = this.CodeExampleText.ResultsPageText;
            this.ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return this.View("example_done");
        }
    }
}
