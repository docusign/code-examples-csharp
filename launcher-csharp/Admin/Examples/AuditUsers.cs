// <copyright file="AuditUsers.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.Admin.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using DocuSign.Admin.Api;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Model;

    public class AuditUsers
    {
        /// <summary>Gets modified users</summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="orgId">DocuSign Organization Id (GUID)</param>
        /// <param name="accountId">The DocuSign Account Id (GUID)</param>
        /// <returns>List of users' data that can be used for auditing purposes</returns>
        public static IEnumerable<UserDrilldownResponse> GetRecentlyModifiedUsersData(string basePath, string accessToken, Guid? accountId, Guid? orgId)
        {
            //ds-snippet-start:Admin5Step2
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:Admin5Step2

            //ds-snippet-start:Admin5Step3
            UsersApi usersApi = new UsersApi(apiClient);
            int tenDaysAgo = -10;
            var getUsersOptions = new UsersApi.GetUsersOptions
            {
                accountId = accountId,
                lastModifiedSince = DateTime.Today.AddDays(tenDaysAgo).ToString("yyyy-MM-dd"),
            };

            var recentlyModifiedUsers = usersApi.GetUsersWithHttpInfo(orgId, getUsersOptions);
            recentlyModifiedUsers.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            recentlyModifiedUsers.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);
            //ds-snippet-end:Admin5Step3

            //ds-snippet-start:Admin5Step5
            var usersData = new List<UserDrilldownResponse>();
            //ds-snippet-end:Admin5Step5
            //ds-snippet-start:Admin5Step4
            foreach (var user in recentlyModifiedUsers.Data.Users)
            {
                var getUserProfilesOptions = new UsersApi.GetUserProfilesOptions { email = user.Email };
                //ds-snippet-end:Admin5Step4
                //ds-snippet-start:Admin5Step5
                var users = usersApi.GetUserProfilesWithHttpInfo(orgId, getUserProfilesOptions);
                users.Headers.TryGetValue("X-RateLimit-Remaining", out remaining);
                users.Headers.TryGetValue("X-RateLimit-Reset", out reset);

                resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
                usersData.AddRange(users.Data.Users);
                //ds-snippet-end:Admin5Step5
            }

            return usersData;
        }
    }
}
