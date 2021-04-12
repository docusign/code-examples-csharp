using System.Collections.Generic;
using System.Linq;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;

namespace DocuSign.CodeExamples.Controllers
{
	[Area("eSignature")]
	[Route("Eg026")]
	public class Eg026PermissionChangeSingleSettingController : EgController
	{
		public Eg026PermissionChangeSingleSettingController(DSConfiguration config, IRequestItemsService requestItemsService) :
			base(config, requestItemsService)
		{
		}

		public override string EgName => "Eg026";

		[BindProperty]
		public List<PermissionProfile> PermissionProfiles { get; set; }

		[BindProperty]
		public PermissionProfileModel ProfileModel { get; set; }

		protected override void InitializeInternal()
		{
			// Data for this method
			// signerEmail 
			// signerName
			var basePath = RequestItemsService.Session.BasePath + "/restapi";
			var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
			var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
			var apiClient = new ApiClient(basePath);
			apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

			var accountsApi = new AccountsApi(apiClient);
			var permissions = accountsApi.ListPermissions(accountId);
			PermissionProfiles = permissions.PermissionProfiles;
			var permissionProfile = permissions.PermissionProfiles.FirstOrDefault();
			ProfileModel = new PermissionProfileModel
			{
				ProfileId = permissionProfile.PermissionProfileId,
				ProfileName = permissionProfile.PermissionProfileName,
				AccountRoleSettingsModel = new AccountRoleSettingsModel(permissionProfile.Settings)
			};
		}

		[HttpPost]
		[Route("Create")]
		public IActionResult Create(PermissionProfileModel profileModel)
		{
			// Check the token with minimal buffer time.
			bool tokenOk = CheckToken(3);
			if (!tokenOk)
			{
				// We could store the parameters of the requested operation so it could be 
				// restarted automatically. But since it should be rare to have a token issue
				// here, we'll make the user re-enter the form data after authentication.
				RequestItemsService.EgName = EgName;
				return Redirect("/ds/mustAuthenticate");
			}

			// Data for this method
			// signerEmail 
			// signerName
			var basePath = RequestItemsService.Session.BasePath + "/restapi";

			// Obtain your OAuth token
			var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
			var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}
			
			try
			{
				// Call the eSignature REST API
				var results = ChangePermissionSingleSetting.UpdatePermissionProfile(
                    profileModel.ProfileId, accessToken, basePath, accountId);

				ViewBag.h1 = "The permission profile was updated";
				ViewBag.message = "The permission profile was updated!<br />Permission profile ID: " + results.PermissionProfileId + ".";
				return View("example_done");
			}
			catch(ApiException apiException)
			{
				ViewBag.errorCode = apiException.ErrorCode;
				ViewBag.errorMessage = apiException.Message;
				return View("Error");
			}
		}
	}
}