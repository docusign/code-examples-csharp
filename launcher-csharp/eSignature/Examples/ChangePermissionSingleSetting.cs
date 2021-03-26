using System.Linq;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;


namespace eSignature.Examples
{
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
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            AccountsApi accountsApi = new AccountsApi(apiClient);

            // Construct the request body
            var permission = accountsApi.ListPermissions(accountId).PermissionProfiles.
                FirstOrDefault(profile => profile.PermissionProfileId == profileId);         

            // Call the eSignature REST API
            return accountsApi.UpdatePermissionProfile(accountId, profileId, permission);
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
