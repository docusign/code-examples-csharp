using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DocuSign.CodeExamples.Admin.Examples;
using DocuSign.Admin.Client;
using System.IO;

namespace DocuSign.CodeExamples.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/Eg03")]
    public class Eg03BulkExportUserDataController : EgController
    {
        public Eg03BulkExportUserDataController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService)
            : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg03";

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
                ViewBag.h1 = "Bulk export user data";
                ViewBag.message = $"User data exported to {filePath}.<br/> Results from UserExport:getUserListExport method:";
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
