using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DocuSign.CodeExamples.Admin.Examples;
using DocuSign.Admin.Client;
using DocuSign.Admin.Model;
using System;
using DocuSign.CodeExamples.eSignature.Models;

namespace DocuSign.CodeExamples.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/Eg04")]
    public class Eg04AddUsersBulkImportController : EgController
    {
        private CodeExampleText codeExampleText;

        public Eg04AddUsersBulkImportController(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 4;

        public override string EgName => "Eg04";

        [MustAuthenticate]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create()
        {
            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.AdminApiBasePath;
            var accountId = RequestItemsService.Session.AccountId;
            var organizationId = RequestItemsService.OrganizationId;
       
            try
            {
                // Call the Admin API to create a new user
                OrganizationImportResponse organizationImportResponse = ImportUser.CreateBulkImportRequest(
                    accessToken, basePath, accountId, organizationId, Config.docCsv);

                //Show results
                ViewBag.h1 = codeExampleText.ResultsPageHeader;
                ViewBag.message = codeExampleText.ResultsPageText;
                ViewBag.Locals.Json = JsonConvert.SerializeObject(organizationImportResponse, Formatting.Indented);
                ViewBag.AdditionalLinkText = codeExampleText.AdditionalPages[0].Name;
                ViewBag.AdditionalLink = "CheckStatus?id=" + organizationImportResponse.Id;

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }

        [MustAuthenticate]
        [HttpGet]
        [Route("CheckStatus")]
        public ActionResult CheckStatus(string id)
        {
            try
            {
                // Obtain your OAuth token
                var accessToken = RequestItemsService.User.AccessToken;
                var basePath = RequestItemsService.Session.AdminApiBasePath;
                var organizationId = RequestItemsService.OrganizationId;
                OrganizationImportResponse organizationImportResponse = ImportUser.CheckkStatus(accessToken, basePath, organizationId, Guid.Parse(id));

                //Show results
                ViewBag.h1 = codeExampleText.AdditionalPages[0].ResultsPageHeader;
                ViewBag.message = codeExampleText.AdditionalPages[0].ResultsPageText;
                ViewBag.Locals.Json = JsonConvert.SerializeObject(organizationImportResponse, Formatting.Indented);
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
