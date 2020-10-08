using System.Collections.Generic;
using System.Linq;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

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

			// Step 1. Obtain your OAuth token
			var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
			var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

			// Step 2. Construct your API headers
			var apiClient = new ApiClient(basePath);
			apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
			var accountsApi = new AccountsApi(apiClient);
			
			// Step 3. Construct the request body
			var permission = accountsApi.ListPermissions(accountId).PermissionProfiles.
				FirstOrDefault(profile => profile.PermissionProfileId == profileModel.ProfileId);
			var settingsToUpdate = new AccountRoleSettingsExtension { SigningUiVersion = "2" };

			settingsToUpdate.UseNewDocuSignExperienceInterface = profileModel.AccountRoleSettingsModel.UseNewDocuSignExperienceInterface? "1" : "0";
			settingsToUpdate.EnableSequentialSigningInterface = profileModel.AccountRoleSettingsModel.EnableSequentialSigningInterface.ToString();
			settingsToUpdate.PowerFormRole = profileModel.AccountRoleSettingsModel.PowerFormRole.ToString();
			settingsToUpdate.VaultingMode = profileModel.AccountRoleSettingsModel.VaultingMode.ToString();
			settingsToUpdate.AllowTaggingInSendAndCorrect = profileModel.AccountRoleSettingsModel.AllowTaggingInSendAndCorrect.ToString();
			settingsToUpdate.AllowedAddressBookAccess = profileModel.AccountRoleSettingsModel.AllowedAddressBookAccess.ToString();
			settingsToUpdate.AllowedTemplateAccess = profileModel.AccountRoleSettingsModel.AllowedTemplateAccess.ToString();

			// Present on PermissionProfileView.
			settingsToUpdate.AllowBulkSending = profileModel.AccountRoleSettingsModel.AllowBulkSending.ToString();
			settingsToUpdate.AllowEnvelopeSending = profileModel.AccountRoleSettingsModel.AllowEnvelopeSending.ToString();
			settingsToUpdate.AllowSignerAttachments = profileModel.AccountRoleSettingsModel.AllowSignerAttachments.ToString();
			settingsToUpdate.AllowApiAccess = profileModel.AccountRoleSettingsModel.AllowApiAccess.ToString();
			settingsToUpdate.AllowApiAccessToAccount = profileModel.AccountRoleSettingsModel.AllowApiAccessToAccount.ToString();
			settingsToUpdate.AllowApiSequentialSigning = profileModel.AccountRoleSettingsModel.AllowApiSequentialSigning.ToString();
			settingsToUpdate.EnableApiRequestLogging = profileModel.AccountRoleSettingsModel.EnableApiRequestLogging.ToString();
			settingsToUpdate.AllowApiSendingOnBehalfOfOthers = profileModel.AccountRoleSettingsModel.AllowApiSendingOnBehalfOfOthers.ToString();
			
			settingsToUpdate.AllowWetSigningOverride = profileModel.AccountRoleSettingsModel.AllowWetSigningOverride.ToString();
			settingsToUpdate.EnableRecipientViewingNotifications = profileModel.AccountRoleSettingsModel.EnableRecipientViewingNotifications.ToString();
			settingsToUpdate.ReceiveCompletedSelfSignedDocumentsAsEmailLinks = profileModel.AccountRoleSettingsModel.ReceiveCompletedSelfSignedDocumentsAsEmailLinks.ToString();
			settingsToUpdate.UseNewSendingInterface = profileModel.AccountRoleSettingsModel.UseNewSendingInterface.ToString();
			settingsToUpdate.AllowDocuSignDesktopClient = profileModel.AccountRoleSettingsModel.AllowDocuSignDesktopClient.ToString();
			settingsToUpdate.AllowSendersToSetRecipientEmailLanguage = profileModel.AccountRoleSettingsModel.AllowSendersToSetRecipientEmailLanguage.ToString();
			settingsToUpdate.AllowVaulting = profileModel.AccountRoleSettingsModel.AllowVaulting.ToString();
			settingsToUpdate.AllowedToBeEnvelopeTransferRecipient = profileModel.AccountRoleSettingsModel.AllowedToBeEnvelopeTransferRecipient.ToString();
			settingsToUpdate.EnableTransactionPointIntegration = profileModel.AccountRoleSettingsModel.EnableTransactionPointIntegration.ToString();
			try
			{
				// Step 4. Call the eSignature REST API
				var results = accountsApi.UpdatePermissionProfile(accountId, profileModel.ProfileId, permission);

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