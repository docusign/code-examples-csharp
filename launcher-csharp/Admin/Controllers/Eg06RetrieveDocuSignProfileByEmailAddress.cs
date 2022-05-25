using DocuSign.Admin.Client;
using DocuSign.Admin.Examples;
using DocuSign.Admin.Model;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace DocuSign.CodeExamples.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/Eg06")]
    public class Eg06RetrieveDocuSignProfileByEmailAddress : EgController
    {
        public Eg06RetrieveDocuSignProfileByEmailAddress(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService)
            : base(dsConfig, requestItemsService)
        {
        }

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

                ViewBag.h1 = "Retrieve the user's DocuSign profile using an email address";
                ViewBag.message = "Results from getDSUserProfile_Email method:";
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