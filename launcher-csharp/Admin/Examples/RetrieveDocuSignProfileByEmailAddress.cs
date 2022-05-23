using DocuSign.Admin.Api;
using DocuSign.Admin.Client;
using DocuSign.Admin.Model;
using System;

namespace DocuSign.Admin.Examples
{
    public class RetrieveDocuSignProfileByEmailAddress
    {
        /// <summary>
        /// Get a DocuSign profile by the email adress
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="orgId">DocuSign Organization Id (GUID)</param>
        /// <param name="email">Email adress (string)</param>
        /// <returns>DocuSign profile, that has the searched email</returns>
        public static UsersDrilldownResponse GetDocuSignProfileByEmailAdress(
            string basePath,
            string accessToken,
            Guid? orgId, 
            string email)
        {
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var usersApi = new UsersApi(apiClient);
            var retrieveUserOptions = new UsersApi.GetUserDSProfilesByEmailOptions
            {
                email = email
            };

            UsersDrilldownResponse userWithSearchedEmail = usersApi.GetUserDSProfilesByEmail(orgId, retrieveUserOptions);

            return userWithSearchedEmail;
        }
    }
}
