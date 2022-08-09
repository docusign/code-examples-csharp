using DocuSign.Admin.Client;
using DocuSign.Admin.Examples;
using DocuSign.Admin.Model;
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
    [Route("[area]/Eg06")]
    public class Eg06RetrieveDocuSignProfileByEmailAddress : EgController
    {
        private CodeExampleText codeExampleText;

        public Eg06RetrieveDocuSignProfileByEmailAddress(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 6;

        public override string EgName => "Eg06";

        [MustAuthenticate]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RetriveProfileByEmail(string email)
        {
            try
            {
                Guid? organizationId = RequestItemsService.OrganizationId;
                string accessToken = RequestItemsService.User.AccessToken;
                string basePath = RequestItemsService.Session.AdminApiBasePath;

                UsersDrilldownResponse profileWithSearchedEmail = RetrieveDocuSignProfileByEmailAddress.
                    GetDocuSignProfileByEmailAdress(basePath, accessToken, organizationId, email);

                ViewBag.h1 = codeExampleText.ResultsPageHeader;
                ViewBag.message = codeExampleText.ResultsPageText;
                ViewBag.Locals.Json = JsonConvert.SerializeObject(profileWithSearchedEmail, Formatting.Indented);

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