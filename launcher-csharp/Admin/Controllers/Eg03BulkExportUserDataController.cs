using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DocuSign.CodeExamples.Admin.Examples;
using DocuSign.Admin.Client;
using System.IO;
using DocuSign.CodeExamples.eSignature.Models;
using System;

namespace DocuSign.CodeExamples.Admin.Controllers
{
    [Area("Admin")]
    [Route("Aeg03")]
    public class Eg03BulkExportUserDataController : EgController
    {
        public Eg03BulkExportUserDataController(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 3;

        public override string EgName => "Aeg03";

        [MustAuthenticate]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create()
        {
            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.AdminApiBasePath;
            var organizationId = RequestItemsService.OrganizationId;
            var filePath = Path.GetFullPath(Path.Combine(
                Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory),
                Config.exportUsersPath));

            try
            {
                // Call the Admin API to create a new user
                var organizationExportsResponse = BulkExportUserData.CreateBulkExportRequest(
                    accessToken, basePath, organizationId, filePath);

                //Show results
                ViewBag.h1 = codeExampleText.ResultsPageHeader;
                ViewBag.message = String.Format(codeExampleText.ResultsPageText, filePath);
                ViewBag.Locals.Json = JsonConvert.SerializeObject(organizationExportsResponse, Formatting.Indented);

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
