using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Monitor.Examples;

namespace DocuSign.CodeExamples.Controllers
{
    [Route("monitorExample001")]
    public class MonitorController : EgController
    {
		private IRequestItemsService _requestItemsService;
		public MonitorController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "How to get monitoring data";
			_requestItemsService = requestItemsService;
		}

        public override string EgName => "monitorExample001";

		[HttpPost]
		public IActionResult Create()
		{
			// Obtain your JWT authentication token
			this._requestItemsService.UpdateUserFromJWT();

			// Preparing data for this method
			string accessToken = RequestItemsService.User.AccessToken;
			string requestPath = "https://lens-d.docusign.net/api/v2.0/datasets/monitor/";

			// Getting monitoring data
			var results = GetMonitoringData.DoWork(accessToken, requestPath);

			// Process results
			ViewBag.h1 = "Response output";
			ViewBag.message = "Monitor data responce output:";
			ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
			return View("example_done");
		}
	}
}