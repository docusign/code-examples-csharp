using System;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Models;
using DocuSign.eSign.Model;
using ESignature.Examples;
using FluentAssertions;
using Xunit;

namespace launcher_csharp.Tests.eSignatureUnitTests
{
    [Collection("eSignature tests")]
    public sealed class CreatePermissionUnitTests
    {
        private const string EsignarurePathPrefix = "/restapi";

        private readonly TestConfig _testConfig;

        public CreatePermissionUnitTests()
        {
            this._testConfig = new TestConfig();

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesApiType.ESignature, _testConfig);
        }

        [Fact]
        public void CreatePermissionProfile_CorrectInputParameters_ReturnsPermissionProfile()
        {
            // Arrange
            string basePath = _testConfig.BasePath + EsignarurePathPrefix;
            var expectedPermissionProfileModel = new PermissionProfileModel
            {
                ProfileName = Guid.NewGuid().ToString("n").Substring(0, 8),
                AccountRoleSettingsModel = new AccountRoleSettingsModel()
            };

            //Act
            PermissionProfile permissionProfile = CreatePermissionProfile.Create(
                expectedPermissionProfileModel.ProfileName,
                PrepareAccountRoleSettings(expectedPermissionProfileModel),
                _testConfig.AccessToken,
                basePath,
                _testConfig.AccountId);

            // Assert
            Assert.NotNull(permissionProfile);
            permissionProfile.PermissionProfileName.Should().Be(expectedPermissionProfileModel.ProfileName);
        }

        public CreatePermissionProfile.AccountRoleSettingsExtension PrepareAccountRoleSettings(PermissionProfileModel permissionProfileModel)
        {
            var accountRoleSettings = new CreatePermissionProfile.AccountRoleSettingsExtension
                {
                    UseNewDocuSignExperienceInterface = "1",
                    EnableSequentialSigningInterface = true.ToString(),
                    PowerFormRole = "admin",
                    VaultingMode = "none",
                    AllowTaggingInSendAndCorrect = true.ToString(),
                    AllowedAddressBookAccess = "personalAndShared",
                    AllowedTemplateAccess = "share",
                    SigningUiVersion = "2",
                    AllowBulkSending = permissionProfileModel.AccountRoleSettingsModel.AllowBulkSending.ToString(),
                    AllowEnvelopeSending = permissionProfileModel.AccountRoleSettingsModel.AllowEnvelopeSending.ToString(),
                    AllowSignerAttachments = permissionProfileModel.AccountRoleSettingsModel.AllowSignerAttachments.ToString(),
                    AllowApiAccess = permissionProfileModel.AccountRoleSettingsModel.AllowApiAccess.ToString(),
                    AllowApiAccessToAccount = permissionProfileModel.AccountRoleSettingsModel.AllowApiAccessToAccount.ToString(),
                    AllowApiSequentialSigning = permissionProfileModel.AccountRoleSettingsModel.AllowApiSequentialSigning.ToString(),
                    EnableApiRequestLogging = permissionProfileModel.AccountRoleSettingsModel.EnableApiRequestLogging.ToString(),
                    AllowApiSendingOnBehalfOfOthers = permissionProfileModel.AccountRoleSettingsModel.AllowApiSendingOnBehalfOfOthers.ToString(),
                    AllowWetSigningOverride = permissionProfileModel.AccountRoleSettingsModel.AllowWetSigningOverride.ToString(),
                    EnableRecipientViewingNotifications = permissionProfileModel.AccountRoleSettingsModel.EnableRecipientViewingNotifications.ToString(),
                    ReceiveCompletedSelfSignedDocumentsAsEmailLinks = permissionProfileModel.AccountRoleSettingsModel.ReceiveCompletedSelfSignedDocumentsAsEmailLinks.ToString(),
                    UseNewSendingInterface = permissionProfileModel.AccountRoleSettingsModel.UseNewSendingInterface.ToString(),
                    AllowDocuSignDesktopClient = permissionProfileModel.AccountRoleSettingsModel.AllowDocuSignDesktopClient.ToString(),
                    AllowSendersToSetRecipientEmailLanguage = permissionProfileModel.AccountRoleSettingsModel.AllowSendersToSetRecipientEmailLanguage.ToString(),
                    AllowVaulting = permissionProfileModel.AccountRoleSettingsModel.AllowVaulting.ToString(),
                    AllowedToBeEnvelopeTransferRecipient = permissionProfileModel.AccountRoleSettingsModel.AllowedToBeEnvelopeTransferRecipient.ToString(),
                    EnableTransactionPointIntegration = permissionProfileModel.AccountRoleSettingsModel.EnableTransactionPointIntegration.ToString()
                };

            return accountRoleSettings;
        }
    }
}
