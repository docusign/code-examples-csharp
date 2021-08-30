using DocuSign.Admin.Examples;
using DocuSign.Admin.Api;
using DocuSign.Admin.Client;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace DocuSign.CodeExamples.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/Eg02")]
    public class Eg02CreateActiveCLMESignUserController : EgController
    {
        private static Guid? clmProductId;
        private static Guid? eSignProductId;
        public Eg02CreateActiveCLMESignUserController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService)
            : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg02";

        protected override void InitializeInternal()
        {
            var organizationId = RequestItemsService.OrganizationId;
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.AdminApiBasePath;
            var accountId = RequestItemsService.Session.AccountId;


            // Step 2 Start
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            // Step 2 End
            
            // Step 3 Start
            var productPermissionProfileApi = new ProductPermissionProfilesApi(apiClient);
            var productPermissionProfiles = productPermissionProfileApi.GetProductPermissionProfiles(organizationId, Guid.Parse(accountId));
            ViewBag.CLMPermissionProfiles = productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "CLM").PermissionProfiles;
            ViewBag.ESignPermissionProfiles = productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "ESign").PermissionProfiles;
            clmProductId = productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "CLM").ProductId;
            eSignProductId = productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "ESign").ProductId;
            // Step 3 End

            // Step 4 Start
            var dsGroupsApi = new DSGroupsApi(apiClient);
            ViewBag.DsGroups = dsGroupsApi.GetDSGroups(organizationId, Guid.Parse(accountId)).DsGroups;
            // Step 4 End
            if (ViewBag.DsGroups.Count == 0)
            {
                throw new ApiException(0, "You do not have any groups set in your DocuSign Admin. Please go to DocuSign Admin and create a group to use this code example.");
            }
        }

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            try
            {
                base.Get();
                return View("Eg02", this);
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }

        [MustAuthenticate]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string userName, string firstName, string lastName, string email, string cLMPermissionProfileId, string eSignPermissionProfileId, string dsGroupId)
        {
            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.AdminApiBasePath;
            var accountId = RequestItemsService.Session.AccountId;
            var orgId = RequestItemsService.OrganizationId;

            try
            {
                var newUser = CreateCLMESignUser.Create(userName, firstName, lastName, email, cLMPermissionProfileId, eSignPermissionProfileId, Guid.Parse(dsGroupId), clmProductId, eSignProductId, basePath, accessToken, Guid.Parse(accountId), orgId);
                ViewBag.h1 = "Create a new active user for CLM and eSignature";
                ViewBag.message = "Results from MultiProductUserManagement:addOrUpdateUser method:";
                ViewBag.Locals.Json = JsonConvert.SerializeObject(newUser, Formatting.Indented);
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
