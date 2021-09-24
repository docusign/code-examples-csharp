using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DocuSign.CodeExamples.Admin.Examples;
using DocuSign.Admin.Client;
using System;

namespace DocuSign.CodeExamples.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/Eg01")]
    public class Eg01CreateUserController: EgController
    {
        public Eg01CreateUserController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService)
            : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg01";

        protected override void InitializeInternal()
        {
            try
            {
                var accessToken = RequestItemsService.User.AccessToken;
                var basePath = RequestItemsService.Session.BasePath + "/restapi";
                var accountId = RequestItemsService.Session.AccountId;

                var (permissionProfiles, groups) = CreateUser.GetPermissionProfilesAndGroups(accessToken, basePath, accountId);

                ViewBag.PermissionProfiles = permissionProfiles.PermissionProfiles;
                ViewBag.Groups = groups.Groups;
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;
            }
        }

        [MustAuthenticate]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string email, string firstName, string lastName, string userName, string permissionProfileId, string groupId)
        {
            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.AdminApiBasePath;
            var accountId = RequestItemsService.Session.AccountId;
            var organizationId = RequestItemsService.OrganizationId;

            try
            {
                // Call the Admin API to create a new user
                var user = CreateUser.CreateNewUser(accessToken, basePath, Guid.Parse(accountId), 
                    organizationId, firstName, lastName, userName, email, Int64.Parse(permissionProfileId), Int64.Parse(groupId));

                //Show results
                ViewBag.h1 = "Create a new eSignature user";
                ViewBag.message = "Results from eSignUserManagement:createUser method:";
                ViewBag.Locals.Json = JsonConvert.SerializeObject(user, Formatting.Indented);

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
