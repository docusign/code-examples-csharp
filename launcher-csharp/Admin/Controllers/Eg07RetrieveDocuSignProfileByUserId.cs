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
    [Route("[area]/Eg07")]
    public class Eg07RetrieveDocuSignProfileByUserId : EgController
    {
        public Eg07RetrieveDocuSignProfileByUserId(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService)
            : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg07";

        [MustAuthenticate]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RetriveProfileById(Guid userId)
        {
            try
            {
                Guid? organizationId = RequestItemsService.OrganizationId;
                string accessToken = RequestItemsService.User.AccessToken;
                string basePath = RequestItemsService.Session.AdminApiBasePath;

                UsersDrilldownResponse usersData = RetrieveDocuSignProfileByUserId.
                    GetDocuSignProfileByUserId(basePath, accessToken, organizationId, userId);

                ViewBag.h1 = "Retrieve the user's DocuSign profile using a User ID";
                ViewBag.message = "Results from getDSUserProfile method:";
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