using DocuSign.Admin.Api;
using DocuSign.Admin.Client;
using DocuSign.Admin.Model;
using System;


namespace DocuSign.Admin.Examples
{
    public class RetrieveDocuSignProfileByUserId
    {
        /// <summary>
        /// Get a DocuSign profile by the user ID
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="orgId">DocuSign Organization Id (GUID)</param>
        /// <param name="userId">The DocuSign User Id (GUID)</param>
        /// <returns>DocuSign profile, that has the searched user ID</returns>
        public static UsersDrilldownResponse GetDocuSignProfileByUserId(
            string basePath, 
            string accessToken, 
            Guid? orgId,
            Guid userId)
        {
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var usersApi = new UsersApi(apiClient);
            var recentlyModifiedUsers = usersApi.GetUserDSProfile(orgId, userId);

            return recentlyModifiedUsers;
        }
    }
}
