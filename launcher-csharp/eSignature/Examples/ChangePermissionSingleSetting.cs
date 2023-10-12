// <copyright file="ChangePermissionSingleSetting.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System.Linq;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class ChangePermissionSingleSetting
    {
        /// <summary>
        /// Updates a permission profile
        /// </summary>
        /// <param name="profileId">The profile ID</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>A permission profile</returns>
        public static PermissionProfile UpdatePermissionProfile(string profileId, string accessToken, string basePath, string accountId)
        {
            // Construct your API headers
            //ds-snippet-start:eSign26Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:eSign26Step2
            AccountsApi accountsApi = new AccountsApi(docuSignClient);

            // Construct the request body
            //ds-snippet-start:eSign26Step3
            var permission = accountsApi.ListPermissions(accountId).PermissionProfiles.
                FirstOrDefault(profile => profile.PermissionProfileId == profileId);
            //ds-snippet-end:eSign26Step3

            // Call the eSignature REST API
            //ds-snippet-start:eSign26Step4
            return accountsApi.UpdatePermissionProfile(accountId, profileId, permission);
            //ds-snippet-end:eSign26Step4
        }

        /// <summary>
        /// Temporary subclass for AccountRoleSettings
        /// This class is needed for now until DCM-3905 is ready
        /// </summary>
        public class AccountRoleSettingsExtension : AccountRoleSettings
        {
            [System.Runtime.Serialization.DataMember(Name = "signingUIVersion", EmitDefaultValue = false)]
            public string SigningUiVersion { get; set; }
        }
    }
}
