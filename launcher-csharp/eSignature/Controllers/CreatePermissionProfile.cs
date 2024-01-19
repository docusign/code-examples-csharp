// <copyright file="CreatePermissionProfile.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("Eg024")]
    public class CreatePermissionProfile : EgController
    {
        public CreatePermissionProfile(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(this.EgName, ExamplesApiType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg024";

        [BindProperty]
        public PermissionProfileModel ProfileModel { get; set; }

        [HttpPost]
        [SetViewBag]
        [Route("Create")]
        public IActionResult Create(PermissionProfileModel permissionProfileModel)
        {
            // Check the token with minimal buffer time.
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            // Data for this method
            // signerEmail
            // signerName
            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";

            // Step 1. Obtain your OAuth token
            var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Step 2. Construct your request
            //ds-snippet-start:eSign24Step3
            var accountRoleSettings = new global::ESignature.Examples.CreatePermissionProfile.AccountRoleSettingsExtension();
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
            //ds-snippet-end:eSign24Step3

            try
            {
                // Step 3. Call the eSignature REST API
                var result = global::ESignature.Examples.CreatePermissionProfile.Create(
                    permissionProfileModel.ProfileName,
                    accountRoleSettings,
                    accessToken,
                    basePath,
                    accountId);

                this.ViewBag.h1 = this.CodeExampleText.ExampleName;
                this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, result.PermissionProfileId, result.PermissionProfileName);
                return this.View("example_done");
            }
            catch (ApiException apiException)
            {
                this.ViewBag.errorCode = apiException.ErrorCode;
                this.ViewBag.errorMessage = apiException.Message;
                this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;

                return this.View("Error");
            }
        }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();

            this.ProfileModel = new PermissionProfileModel();
        }
    }
}