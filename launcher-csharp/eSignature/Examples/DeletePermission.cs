using DocuSign.eSign.Api;
using DocuSign.eSign.Client;

namespace eSignature.Examples
{
    public static class DeletePermission
    {
		/// <summary>
		/// Deletes a permission profile
		/// </summary>
		/// <param name="permissionProfileId">Permission profile ID</param>
		/// <param name="accessToken">Access Token for API call (OAuth)</param>
		/// <param name="basePath">BasePath for API calls (URI)</param>
		/// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
		public static void DeletePermissionProfile(string permissionProfileId, string accessToken, string basePath, string accountId)
		{
			// Construct your API headers
			var apiClient = new ApiClient(basePath);
			apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
			AccountsApi accountsApi = new AccountsApi(apiClient);
			// Call the eSignature REST API
			accountsApi.DeletePermissionProfile(accountId, permissionProfileId);
		}
	}
}

