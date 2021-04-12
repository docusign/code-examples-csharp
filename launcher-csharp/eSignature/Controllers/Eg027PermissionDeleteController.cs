using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using eSignature.Examples;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("Eg027")]
    public class Eg027PermissionDeleteController : EgController
    {
        public Eg027PermissionDeleteController(DSConfiguration config, IRequestItemsService requestItemsService) :
        base(config, requestItemsService)
        {
        }

        public override string EgName => "Eg027";

        protected override void InitializeInternal()
        {
            // Data for this method
            // permission profiles
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Get all available permissions
            var accountsApi = new AccountsApi(apiClient);
            var permissions = accountsApi.ListPermissions(accountId);
            ViewBag.PermissionProfiles =
            permissions.PermissionProfiles.Select(pr => new SelectListItem
            {
                Text = pr.PermissionProfileName,
                Value = pr.PermissionProfileId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string permissionProfileId)
        {
            // Check the minimal buffer time.
            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            // Uri of rest api
            var basePath = RequestItemsService.Session.BasePath + "/restapi";

            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                //Step 3. Call the eSignature REST API 
                DeletePermission.DeletePermissionProfile(permissionProfileId, accessToken, basePath, accountId);
            }
            catch (ApiException ex)
            {
                //Request failed. Display error text
                ViewBag.h1 = "The permission profile failed to delete";
                ViewBag.message = $"The permission profile failed to delete.<br /> Reason: <br />{ex.ErrorContent}";
                return View("example_done");
            }

            ViewBag.h1 = "The permission profile is deleted";
            ViewBag.message = "The permission profile is deleted!";
            return View("example_done");
        }
    }
}