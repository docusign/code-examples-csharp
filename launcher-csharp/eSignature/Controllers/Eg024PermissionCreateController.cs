using DocuSign.eSign.Client;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using eSignature.Examples;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
	[Route("Eg024")]
	public class Eg024PermissionCreateController : EgController
	{
		public Eg024PermissionCreateController(DSConfiguration config, IRequestItemsService requestItemsService) :
			base(config, requestItemsService)
		{
		}

		public override string EgName => "Eg024";

		[BindProperty]
		public PermissionProfileModel ProfileModel { get; set; }

		protected override void InitializeInternal() => ProfileModel = new PermissionProfileModel();

		[HttpPost]
		[Route("Create")]
		public IActionResult Create(PermissionProfileModel permissionProfileModel)
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

			// Step 2. Construct your request
			var accountRoleSettings = new CreatePermissionProfile.AccountRoleSettingsExtension();
			accountRoleSettings.UseNewDocuSignExperienceInterface = "1";
			accountRoleSettings.EnableSequentialSigningInterface = true.ToString();
			accountRoleSettings.PowerFormRole = "admin";
			accountRoleSettings.VaultingMode = "none";
			accountRoleSettings.AllowTaggingInSendAndCorrect = true.ToString();
			accountRoleSettings.AllowedAddressBookAccess = "personalAndShared";
			accountRoleSettings.AllowedTemplateAccess = "share";
			accountRoleSettings.SigningUiVersion = "2";

			// Present on PermissionProfileView.
			accountRoleSettings.AllowBulkSending = permissionProfileModel.AccountRoleSettingsModel.AllowBulkSending.ToString();
			accountRoleSettings.AllowEnvelopeSending = permissionProfileModel.AccountRoleSettingsModel.AllowEnvelopeSending.ToString();
			accountRoleSettings.AllowSignerAttachments = permissionProfileModel.AccountRoleSettingsModel.AllowSignerAttachments.ToString();
			accountRoleSettings.AllowApiAccess = permissionProfileModel.AccountRoleSettingsModel.AllowApiAccess.ToString();
			accountRoleSettings.AllowApiAccessToAccount = permissionProfileModel.AccountRoleSettingsModel.AllowApiAccessToAccount.ToString();
			accountRoleSettings.AllowApiSequentialSigning = permissionProfileModel.AccountRoleSettingsModel.AllowApiSequentialSigning.ToString();
			accountRoleSettings.EnableApiRequestLogging = permissionProfileModel.AccountRoleSettingsModel.EnableApiRequestLogging.ToString();
			accountRoleSettings.AllowApiSendingOnBehalfOfOthers = permissionProfileModel.AccountRoleSettingsModel.AllowApiSendingOnBehalfOfOthers.ToString();
			accountRoleSettings.AllowWetSigningOverride = permissionProfileModel.AccountRoleSettingsModel.AllowWetSigningOverride.ToString();
			accountRoleSettings.EnableRecipientViewingNotifications = permissionProfileModel.AccountRoleSettingsModel.EnableRecipientViewingNotifications.ToString();
			accountRoleSettings.ReceiveCompletedSelfSignedDocumentsAsEmailLinks = permissionProfileModel.AccountRoleSettingsModel.ReceiveCompletedSelfSignedDocumentsAsEmailLinks.ToString();
			accountRoleSettings.UseNewSendingInterface = permissionProfileModel.AccountRoleSettingsModel.UseNewSendingInterface.ToString();
			accountRoleSettings.AllowDocuSignDesktopClient = permissionProfileModel.AccountRoleSettingsModel.AllowDocuSignDesktopClient.ToString();
			accountRoleSettings.AllowSendersToSetRecipientEmailLanguage = permissionProfileModel.AccountRoleSettingsModel.AllowSendersToSetRecipientEmailLanguage.ToString();
			accountRoleSettings.AllowVaulting = permissionProfileModel.AccountRoleSettingsModel.AllowVaulting.ToString();
			accountRoleSettings.AllowedToBeEnvelopeTransferRecipient = permissionProfileModel.AccountRoleSettingsModel.AllowedToBeEnvelopeTransferRecipient.ToString();
			accountRoleSettings.EnableTransactionPointIntegration = permissionProfileModel.AccountRoleSettingsModel.EnableTransactionPointIntegration.ToString();

			try
			{
				// Step 3. Call the eSignature REST API
				var result = CreatePermissionProfile.Create(permissionProfileModel.ProfileName,
                    accountRoleSettings, accessToken, basePath, accountId);

				ViewBag.h1 = "The permission profile was updated";
				ViewBag.message = $"The permission profile was created!<br />Permission profile ID: {result.PermissionProfileId}, name:{result.PermissionProfileName}.";
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