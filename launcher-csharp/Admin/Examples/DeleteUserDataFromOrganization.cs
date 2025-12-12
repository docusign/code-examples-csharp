// <copyright file="DeleteUserDataFromOrganization.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.Admin.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.Admin.Api;
    using DocuSign.Admin.Client;
    using DocuSign.Admin.Model;

    public class DeleteUserDataFromOrganization
    {
        /// <summary>
        /// Delete user data from organization.
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="organizationId">DocuSign Organization Id (GUID)</param>
        /// <param name="emailAddress">The email address of the user (GUID)</param>
        /// <returns>Result of the delete user action</returns>
        public static IndividualUserDataRedactionResponse DeleteUserDataFromOrganizationByEmail(
            string basePath,
            string accessToken,
            Guid organizationId,
            string emailAddress)
        {
            //ds-snippet-start:Admin11Step2
            var docusignClient = new DocuSignClient(basePath);
            docusignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var usersApi = new UsersApi(docusignClient);
            //ds-snippet-end:Admin11Step2
            var getProfilesOptions = new UsersApi.GetUserDSProfilesByEmailOptions
            {
                email = emailAddress,
            };

            var profiles = usersApi.GetUserDSProfilesByEmailWithHttpInfo(organizationId, getProfilesOptions);
            profiles.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            profiles.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            var user = profiles.Data.Users?[0];

            var organizationsApi = new OrganizationsApi(docusignClient);

            //ds-snippet-start:Admin11Step3
            var userRedactionRequest = new IndividualUserDataRedactionRequest
            {
                UserId = user?.Id,
                Memberships = new List<MembershipDataRedactionRequest>
                {
                    new MembershipDataRedactionRequest
                    {
                        AccountId = user?.Memberships?[0].AccountId,
                    },
                },
            };
            //ds-snippet-end:Admin11Step3

            //ds-snippet-start:Admin11Step4
            var response = organizationsApi.RedactIndividualUserDataWithHttpInfo(organizationId, userRedactionRequest);
            response.Headers.TryGetValue("X-RateLimit-Remaining", out remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            return response.Data;
            //ds-snippet-end:Admin11Step4
        }
    }
}
