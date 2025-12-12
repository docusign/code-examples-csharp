// <copyright file="RetrieveDocuSignProfileByEmailAddress.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.Admin.Examples
{
    using System;
    using DocuSign.Admin.Api;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Model;

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
            //ds-snippet-start:Admin6Step2
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:Admin6Step2

            //ds-snippet-start:Admin6Step3
            var usersApi = new UsersApi(apiClient);
            var retrieveUserOptions = new UsersApi.GetUserDSProfilesByEmailOptions
            {
                email = email,
            };

            var userWithSearchedEmail = usersApi.GetUserDSProfilesByEmailWithHttpInfo(orgId, retrieveUserOptions);
            userWithSearchedEmail.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            userWithSearchedEmail.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            //ds-snippet-end:Admin6Step3
            return userWithSearchedEmail.Data;
        }
    }
}
