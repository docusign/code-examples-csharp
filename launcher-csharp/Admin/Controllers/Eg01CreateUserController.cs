using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DocuSign.CodeExamples.Admin.Examples;
using DocuSign.OrgAdmin.Client;
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

        [MustAuthenticate]
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string email, string firstName, string lastName, string userName)
        {
            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.AdminApiBasePath;
            var accountId = RequestItemsService.Session.AccountId;
            var organizationId = RequestItemsService.OrganizationId;
            const long permissionProfileId = 10747593;
            const long groupId = 7593368;

            try
            {
                // Call the Admin API to create a new user
                var user = CreateUser.CreateNewUser(accessToken, basePath, Guid.Parse(accountId), 
                    organizationId, firstName, lastName, userName, email, permissionProfileId, groupId);

                //Show results
                ViewBag.h1 = "The user was successfully created";
                ViewBag.message = $"The user was created! User ID: {user.Id}";
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
