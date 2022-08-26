using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Monitor.Examples;
using DocuSign.CodeExamples.Monitor.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("Monitor")]
    [Route("monitorExample002")]
    public class WebQueryEndpointFunc : EgController
    {
        private IRequestItemsService _requestItemsService;

        private readonly Monitor.Examples.WebQueryEndpointFunc _webQueryEndpointFunc = new Monitor.Examples.WebQueryEndpointFunc();

        public WebQueryEndpointFunc(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "Post web query";
            _requestItemsService = requestItemsService;
        }

        public override string EgName => "monitorExample002";

        [BindProperty]
        public MonitorFilterModel MonitorFilterModel { get; set; }

        protected override void InitializeInternal()
        {
            MonitorFilterModel = new MonitorFilterModel();
        }

        [MustAuthenticate]
        [HttpPost]
        public IActionResult Create(MonitorFilterModel monitorFilterModel)
        {
            // Obtain your JWT authentication token
            this._requestItemsService.UpdateUserFromJWT();

            var filterStartDate = monitorFilterModel.FieldDataChangedStartDate.ToString(CultureInfo.InvariantCulture);
            var filterEndDate = monitorFilterModel.FieldDataChangedEndDate.ToString(CultureInfo.InvariantCulture);

            // Preparing data for this method
            string accessToken = RequestItemsService.User.AccessToken;
            string accountId = RequestItemsService.Session.AccountId;
            string requestPath = "https://lens-d.docusign.net/api/v2.0/datasets/monitor/";

            // Post web query method call
            var results = _webQueryEndpointFunc.Invoke(
                accessToken, 
                requestPath, 
                accountId,
                filterStartDate,
                filterEndDate);

            // Process results
            ViewBag.h1 = "Query monitoring data with filters";
            ViewBag.message = "Results from DataSet:postWebQuery method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return View("example_done");
        }
    }
}
