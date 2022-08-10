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
using System.Linq;
using DocuSign.CodeExamples.eSignature.Models;

namespace DocuSign.CodeExamples.Admin.Controllers
{
    [Area("Admin")]
    [Route("Aeg09")]
    public class Eg09DeleteUserProductPermissionProfile : EgController
    {
        public static Dictionary<Guid?, string> products;

        public static Guid clmProductId = Guid.Parse("37f013eb-7012-4588-8028-357b39fdbd00");

        public static Guid eSignatureProductId = Guid.Parse("f6406c68-225c-4e9b-9894-64152a26fa83");

        public const string CLM_PROFILES_NOT_FOUND = "No CLM permission profiles are connected to this user";

        public const string ESIGN_PROFILES_NOT_FOUND = "No eSignature permission profiles are connected to this user";

        public Eg09DeleteUserProductPermissionProfile(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 9;

        public override string EgName => "Aeg09";

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            Guid? organizationId = RequestItemsService.OrganizationId;
            string accessToken = RequestItemsService.User.AccessToken;
            string basePath = RequestItemsService.Session.AdminApiBasePath;
            Guid accountId = Guid.Parse(RequestItemsService.Session.AccountId);
            products = new Dictionary<Guid?, string>();

            if (RequestItemsService.EmailAddress != null)
            {
                var profiles = DeleteUserProductPermissionProfileById.GetPermissionProfilesByEmail(
                    basePath, 
                    accessToken, 
                    organizationId, 
                    accountId, 
                    RequestItemsService.EmailAddress
                );

                var clmProfiles = profiles.ProductPermissionProfiles
                    .Find(x => x.ProductName == "CLM")?
                    .PermissionProfiles
                        .Select(x => x.PermissionProfileName).ToList();

                var eSignProfiles = profiles.ProductPermissionProfiles
                    .Find(x => x.ProductName == "ESign")?
                    .PermissionProfiles?
                        .Select(x => x.PermissionProfileName).ToList();

                var clmProfilesFormatted = clmProfiles != null 
                    ? string.Join(",", clmProfiles) 
                    : CLM_PROFILES_NOT_FOUND;

                var eSignProfilesFormatted = eSignProfiles != null 
                    ? string.Join(",", eSignProfiles) 
                    : ESIGN_PROFILES_NOT_FOUND;

                products.TryAdd(clmProductId, string.Format("CLM - {0}", clmProfilesFormatted));
                products.TryAdd(eSignatureProductId, string.Format("eSignature - {0}", eSignProfilesFormatted));

                ViewBag.Products = products;
            }

            ViewBag.EmailAddress = RequestItemsService.EmailAddress;
        }

        [MustAuthenticate]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserProductProfile(string productId)
        {
            try
            {
                Guid? organizationId = RequestItemsService.OrganizationId;
                string accessToken = RequestItemsService.User.AccessToken;
                string basePath = RequestItemsService.Session.AdminApiBasePath;
                Guid accountId = Guid.Parse(RequestItemsService.Session.AccountId);
                string email = RequestItemsService.EmailAddress;

                RemoveUserProductsResponse response = DeleteUserProductPermissionProfileById.DeleteUserProductPermissionProfile(
                    basePath, 
                    accessToken, 
                    organizationId,
                    accountId, 
                    email, 
                    Guid.Parse(productId)
                );

                ViewBag.h1 = codeExampleText.ResultsPageHeader;
                ViewBag.message = codeExampleText.ResultsPageText;
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