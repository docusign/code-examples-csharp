using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DocuSign.CodeExamples.Admin.Examples;
using DocuSign.Admin.Client;

namespace DocuSign.CodeExamples.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/Eg04")]
    public class Eg04AddUsersBulkImportController : EgController
    {
        public Eg04AddUsersBulkImportController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService)
            : base(dsConfig, requestItemsService)
        {
        }

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
            string csvFileData = $"AccountID, AccountName, FirstName, LastName, UserEmail, eSignPermissionProfile, Group, Language, UserTitle, CompanyName, AddressLine1, AddressLine2, City, StateRegionProvince, PostalCode, Phone, LoginPolicy, AutoActivate\n" +
                $"{accountId},Sample Account, First1, Last1, example@sampleemail.example,Account Administrator, Everyone, en, Mr., Some Division,123 4th St, Suite C1,Seattle,WA,8178,2065559999,fedAuthRequired,";

            try
            {
                // Call the Admin API to create a new user
                var organizationImportResponse = ImportUser.CreateBulkImportRequest(
                    accessToken, basePath, organizationId, csvFileData);

                //Show results
                ViewBag.h1 = "Results of bulk-export";
                ViewBag.message = $"User data imported. Bulk-import response:";
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
