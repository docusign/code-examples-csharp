using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.eSignature.Models;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Rooms.Models;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Examples;
using DocuSign.Rooms.Model;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    [Area("Rooms")]
    [Route("Eg08")]
    public class Eg08GrantOfficeAccessToFormGroupController : EgController
    {
        public Eg08GrantOfficeAccessToFormGroupController(
            DSConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService) : base(dsConfig, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 8;

        public override string EgName => "Eg08";

        [BindProperty]
        public OfficeAccessModel OfficeAccessModel { get; set; }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            OfficeAccessModel = new OfficeAccessModel();
        }

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            base.Get();

            // Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to get offices and form groups
                (OfficeSummaryList offices, FormGroupSummaryList formGroups) = 
                    GrantOfficeAccessToFormGroup.GetOfficesAndFormGroups(basePath, accessToken, accountId);

                OfficeAccessModel = new OfficeAccessModel { Offices = offices.OfficeSummaries, FormGroups = formGroups.FormGroups };

                return View("Eg08", this);
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }

        [MustAuthenticate]
        [Route("GrantAccess")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GrantAccess(OfficeAccessModel roomDocumentModel)
        {
            // Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to grant office access to a form group
                GrantOfficeAccessToFormGroup.GrantAccess(basePath, accessToken, accountId, roomDocumentModel.FormGroupId, roomDocumentModel.OfficeId);

                ViewBag.h1 = codeExampleText.ResultsPageHeader;
                ViewBag.message = string.Format(
                    codeExampleText.ResultsPageText, 
                    roomDocumentModel.OfficeId, 
                    roomDocumentModel.FormGroupId);

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }
    }
}
