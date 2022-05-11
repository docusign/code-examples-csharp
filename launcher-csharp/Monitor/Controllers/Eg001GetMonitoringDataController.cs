using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Monitor.Examples;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("Monitor")]
    [Route("monitorExample001")]
    public class Eg001GetMonitoringDataController : EgController
    {
        private IRequestItemsService _requestItemsService;

        private readonly GetMonitoringDataFunc _getMonitoringDataFunc = new GetMonitoringDataFunc();

        public Eg001GetMonitoringDataController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "How to get monitoring data";
            _requestItemsService = requestItemsService;
        }

        public override string EgName => "monitorExample001";

        [MustAuthenticate]
        [HttpPost]
        public IActionResult Create()
        {
            // Obtain your JWT authentication token
            this._requestItemsService.UpdateUserFromJWT();

            // Preparing data for this method
            string accessToken = RequestItemsService.User.AccessToken;
            string requestPath = "https://lens-d.docusign.net/api/v2.0/datasets/monitor/";

            // Getting monitoring data
            var results = _getMonitoringDataFunc.Invoke(accessToken, requestPath);

            // Process results
            ViewBag.h1 = "Get monitoring data";
            ViewBag.message = "Results from DataSet:getStream method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return View("example_done");
        }
    }
}
