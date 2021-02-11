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
    [Route("Eg025")]
    public class Eg025PermissionSetUserGroupController : EgController
    {
        public Eg025PermissionSetUserGroupController(DSConfiguration config, IRequestItemsService requestItemsService) :
            base(config, requestItemsService)
        {
        }

        public override string EgName => "Eg025";

        protected override void InitializeInternal()
        {
            // Data for this method
            // Permission profiles
            // User groups
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var accountsApi = new AccountsApi(apiClient);
            var groupsApi = new GroupsApi(apiClient);
            var permissions = accountsApi.ListPermissions(accountId);
            var userGroups = groupsApi.ListGroups(accountId);

            // List all available permission profiles
            ViewBag.PermissionProfiles =
                permissions.PermissionProfiles.Select(pr => new SelectListItem
                {
                    Text = pr.PermissionProfileName,
                    Value = pr.PermissionProfileId
                });

            // List all available user groups
            ViewBag.UserGroups =
                userGroups.Groups.Select(pr => new SelectListItem
                {
                    Text = $"{pr.GroupName}",
                    Value = pr.GroupId
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetProfile(string permissionProfileId, string userGroupId)
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

            // Call the Examples API method to set permissions to the specified user group
            var result = SetUserGroupPermission.GetGroupInformation(permissionProfileId, userGroupId, 
                accessToken, basePath, accountId);

            var errorDetails = result.Groups.FirstOrDefault()?.ErrorDetails;

            if (errorDetails is null)
            {
                ViewBag.h1 = "The permission profile was successfully set to the user group";
                ViewBag.message = "The permission profile was successfully set to the user group!";
            }
            else
            {
                ViewBag.h1 = "The permission profile failed to set to the user group";
                ViewBag.message = "The permission profile failed to set to the user group.<br /> " +
                                  $"Reason: {errorDetails.Message}";
            }

            return View("example_done");
        }
    }
}