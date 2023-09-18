// <copyright file="GetMonitoringData.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System.Linq;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using DocuSign.CodeExamples.Monitor.Examples;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Monitor")]
    [Route("meg001")]
    public class GetMonitoringData : EgController
    {
        private readonly Monitor.Examples.GetMonitoringDataFunc getMonitoringDataFunc = new Monitor.Examples.GetMonitoringDataFunc();

        private readonly IRequestItemsService requestItemsService;

        public GetMonitoringData(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.requestItemsService = requestItemsService;

            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.Monitor);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "meg001";

        [MustAuthenticate]
        [SetViewBag]
        [HttpPost]
        public IActionResult Create()
        {
            // Obtain your JWT authentication token
            this.requestItemsService.UpdateUserFromJwt();

            // Preparing data for this method
            string accessToken = this.RequestItemsService.User.AccessToken;
            string requestPath = "https://lens-d.docusign.net/api/v2.0/datasets/monitor/";

            // Getting monitoring data
            var results = this.getMonitoringDataFunc.Invoke(accessToken, requestPath);

            if (results.FirstOrDefault() as string == "ERROR")
            {
                this.ViewBag.fixingInstructions = (string)results.LastOrDefault();
                this.ViewBag.errorCode = "No Monitor Enabled";
                return this.View("Error");
            }
            else
            {
                // Process results
                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = this.CodeExampleText.ResultsPageText;
                this.ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
                return this.View("example_done");
            }
        }
    }
}
