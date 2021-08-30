using DocuSign.Admin.Api;
using DocuSign.Admin.Client;
using DocuSign.Admin.Model;
using System;
using System.Collections.Generic;


namespace DocuSign.Admin.Examples
{
    public class AuditUsers
    {
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="orgId">DocuSign Organization Id (GUID)</param>
        /// <param name="accountId">The DocuSign Account Id (GUID)</param>
        /// <returns>List of users' data that can be used for auditing purposes</returns>
        public static IEnumerable<UserDrilldownResponse> GetRecentlyModifiedUsersData(string basePath, string accessToken, Guid? accountId, Guid? orgId)
        {
            // Step 2 start
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            // Step 2 end
            // Step 3 start
            UsersApi usersApi = new UsersApi(apiClient);
            var getUsersOptions = new UsersApi.GetUsersOptions { accountId = accountId, lastModifiedSince = DateTime.Today.AddDays(-10).ToShortDateString() };
            var recentlyModifiedUsers = usersApi.GetUsers(orgId, getUsersOptions);
            // Step 3 end
            // Step 5 start
            var usersData = new List<UserDrilldownResponse>();
            foreach (var user in recentlyModifiedUsers.Users)
            {
                var getUserProfilesOptions = new UsersApi.GetUserProfilesOptions { email = user.Email };
                usersData.AddRange(usersApi.GetUserProfiles(orgId, getUserProfilesOptions).Users);
            }
            // Step 5 end
            return usersData;
        }
    }
}
