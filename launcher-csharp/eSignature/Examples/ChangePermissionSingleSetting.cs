// <copyright file="ChangePermissionSingleSetting.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
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
            var response = accountsApi.ListPermissionsWithHttpInfo(accountId);

            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            var permission = response.Data.PermissionProfiles.
                FirstOrDefault(profile => profile.PermissionProfileId == profileId);
            //ds-snippet-end:eSign26Step3

            // Call the eSignature REST API
            //ds-snippet-start:eSign26Step4
            var permissionProfile = accountsApi.UpdatePermissionProfileWithHttpInfo(accountId, profileId, permission);

            response.Headers.TryGetValue("X-RateLimit-Remaining", out remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            return permissionProfile.Data;
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
