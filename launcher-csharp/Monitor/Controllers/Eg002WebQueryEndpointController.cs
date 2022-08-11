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
    public class Eg002WebQueryEndpointController : EgController
    {
        private IRequestItemsService _requestItemsService;

        private readonly WebQueryEndpointFunc _webQueryEndpointFunc = new WebQueryEndpointFunc();

        public Eg002WebQueryEndpointController(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            _requestItemsService = requestItemsService;
            
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 2;

        public override string EgName => "monitorExample002";

        [BindProperty]
        public MonitorFilterModel MonitorFilterModel { get; set; }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();

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
            ViewBag.h1 = codeExampleText.ResultsPageHeader;
            ViewBag.message = codeExampleText.ResultsPageText;
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return View("example_done");
        }
    }
}
