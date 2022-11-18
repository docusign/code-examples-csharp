// <copyright file="Eg002WebQueryEndpointController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System.Globalization;
    using System.Linq;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using DocuSign.CodeExamples.Monitor.Examples;
    using DocuSign.CodeExamples.Monitor.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Area("Monitor")]
    [Route("monitorExample002")]
    public class WebQueryEndpointFunc : EgController
    {
        private readonly Monitor.Examples.WebQueryEndpointFunc webQueryEndpointFunc = new Monitor.Examples.WebQueryEndpointFunc();
        private IRequestItemsService requestItemsService;

        public WebQueryEndpointFunc(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.requestItemsService = requestItemsService;

            this.CodeExampleText = this.GetExampleText(EgName, ExamplesAPIType.Monitor);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "monitorExample002";

        [BindProperty]
        public MonitorFilterModel MonitorFilterModel { get; set; }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();

            this.MonitorFilterModel = new MonitorFilterModel();
        }

        [MustAuthenticate]
        [SetViewBag]
        [HttpPost]
        public IActionResult Create(MonitorFilterModel monitorFilterModel)
        {
            // Obtain your JWT authentication token
            this.requestItemsService.UpdateUserFromJWT();

            var filterStartDate = monitorFilterModel.FieldDataChangedStartDate.ToString(CultureInfo.InvariantCulture);
            var filterEndDate = monitorFilterModel.FieldDataChangedEndDate.ToString(CultureInfo.InvariantCulture);

            // Preparing data for this method
            string accessToken = this.RequestItemsService.User.AccessToken;
            string accountId = this.RequestItemsService.Session.AccountId;
            string requestPath = "https://lens-d.docusign.net/api/v2.0/datasets/monitor/";

            // Post web query method call
            var results = this.webQueryEndpointFunc.Invoke(
                accessToken,
                requestPath,
                accountId,
                filterStartDate,
                filterEndDate);

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
