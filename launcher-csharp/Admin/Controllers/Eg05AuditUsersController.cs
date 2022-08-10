using DocuSign.Admin.Examples;
using DocuSign.Admin.Client;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using DocuSign.CodeExamples.eSignature.Models;

namespace DocuSign.CodeExamples.Admin.Controllers
{
    [Area("Admin")]
    [Route("Aeg05")]
    public class Eg05AuditUsersController : EgController
    {
        public Eg05AuditUsersController(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 5;

        public override string EgName => "Aeg05";
        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            ViewBag.AccountId = RequestItemsService.Session.AccountId;
        }


        [MustAuthenticate]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Audit()
        {
            try
            {
                var organizationId = RequestItemsService.OrganizationId;
                var accessToken = RequestItemsService.User.AccessToken;
                var basePath = RequestItemsService.Session.AdminApiBasePath;
                var accountId = RequestItemsService.Session.AccountId;
                var usersData = AuditUsers.GetRecentlyModifiedUsersData(basePath, accessToken, Guid.Parse(accountId), organizationId);
                // Process results
                ViewBag.h1 = codeExampleText.ResultsPageHeader;
                ViewBag.message = codeExampleText.ResultsPageText;
                ViewBag.Locals.Json = JsonConvert.SerializeObject(usersData, Formatting.Indented);
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