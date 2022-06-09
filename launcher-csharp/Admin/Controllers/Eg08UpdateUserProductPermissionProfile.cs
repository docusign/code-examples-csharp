using DocuSign.Admin.Api;
using DocuSign.Admin.Client;
using DocuSign.Admin.Examples;
using DocuSign.Admin.Model;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DocuSign.CodeExamples.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/Eg08")]
    public class Eg08UpdateUserProductPermissionProfile : EgController
    {
        private static ProductPermissionProfilesResponse productPermissionProfiles;

        public static Dictionary<Guid?, string> products = new Dictionary<Guid?, string>();

        public Eg08UpdateUserProductPermissionProfile(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService)
            : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg08";

        protected override void InitializeInternal()
        {
            Guid? organizationId = RequestItemsService.OrganizationId;
            string accessToken = RequestItemsService.User.AccessToken;
            string basePath = RequestItemsService.Session.AdminApiBasePath;
            Guid accountId = Guid.Parse(RequestItemsService.Session.AccountId);

            productPermissionProfiles = UpdateUserProductPermissionProfileByEmail
                .GetPermissionProfiles(basePath, accessToken, organizationId, accountId);

            ViewBag.CLMPermissionProfiles = productPermissionProfiles.ProductPermissionProfiles
                .Find(x => x.ProductName == "CLM").PermissionProfiles;

            products.TryAdd(
                productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "CLM").ProductId,
                "CLM"
            );

            products.TryAdd(
                productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "ESign").ProductId,
                "ESignature"
            );

            ViewBag.Products = products;
            ViewBag.EmailAddress = RequestItemsService.EmailAddress;
        }

        [Route("/getPermissionProfiles")]
        public IActionResult getPermissionProfiles(Guid? productId)
        {
            return Json(productPermissionProfiles.ProductPermissionProfiles
                .Find(x => x.ProductId == productId)?.PermissionProfiles);
        }

        [MustAuthenticate]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUserProductPermissionProfile(string productId, string permissionProfileId)
        {
            try
            {
                Guid? organizationId = RequestItemsService.OrganizationId;
                string accessToken = RequestItemsService.User.AccessToken;
                string basePath = RequestItemsService.Session.AdminApiBasePath;
                Guid accountId = Guid.Parse(RequestItemsService.Session.AccountId);
                string email = RequestItemsService.EmailAddress;

                UserProductPermissionProfilesResponse response = UpdateUserProductPermissionProfileByEmail.UpdateUserProductPermissionProfile(
                    basePath, 
                    accessToken, 
                    organizationId, 
                    accountId, 
                    email, 
                    Guid.Parse(productId), 
                    permissionProfileId
                );

                ViewBag.h1 = "Update user product permission profiles using an email address";
                ViewBag.message = "Results from MultiProductUserManagement:addUserProductPermissionProfilesByEmail method:";
                ViewBag.Locals.Json = JsonConvert.SerializeObject(response, Formatting.Indented);

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