using DocuSign.Rooms.Api;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Model;

namespace DocuSign.Rooms.Examples
{
    public class CreateFormGroups
    {
        /// <summary>
        /// Creates form group
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="groupName">The name of new group</param>
        /// <returns>The new form group</returns>
        public static FormGroup CreateGroup(
            string basePath,
            string accessToken,
            string accountId,
            string groupName)
        {
            // Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var formGroupsApi = new FormGroupsApi(apiClient);

            var formGroupForCreate = new FormGroupForCreate(groupName);

            // Call the Rooms API to create form group
            return formGroupsApi.CreateFormGroup(accountId, formGroupForCreate);
        }
    }
}
