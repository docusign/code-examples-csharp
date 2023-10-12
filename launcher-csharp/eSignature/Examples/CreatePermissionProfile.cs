// <copyright file="CreatePermissionProfile.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class CreatePermissionProfile
    {
        /// <summary>
        /// Creates a permission profile
        /// </summary>
        /// <param name="profileName">Profile name</param>
        /// <param name="accountRoleSettings">Sccount role settings</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>A permission profile</returns>
        public static PermissionProfile Create(string profileName, AccountRoleSettingsExtension accountRoleSettings, string accessToken, string basePath, string accountId)
        {
            // Construct your API headers
            //ds-snippet-start:eSign24Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            AccountsApi accountsApi = new AccountsApi(docuSignClient);
            //ds-snippet-end:eSign24Step2

            var newPermissionProfile = new PermissionProfile(PermissionProfileName: profileName, Settings: accountRoleSettings);

            // Call the eSignature REST API
            //ds-snippet-start:eSign24Step4
            return accountsApi.CreatePermissionProfile(accountId, newPermissionProfile);
            //ds-snippet-end:eSign24Step4
        }

        public class AccountRoleSettingsExtension : AccountRoleSettings
        {
            [System.Runtime.Serialization.DataMember(Name = "signingUIVersion", EmitDefaultValue = false)]
            public string SigningUiVersion { get; set; }
        }
    }
}
