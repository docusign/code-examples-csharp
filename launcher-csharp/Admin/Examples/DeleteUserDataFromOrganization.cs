// <copyright file="DeleteUserDataFromOrganization.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
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
            var docusignClient = new DocuSignClient(basePath);
            docusignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var usersApi = new UsersApi(docusignClient);
            var getProfilesOptions = new UsersApi.GetUserDSProfilesByEmailOptions
            {
                email = emailAddress,
            };

            UsersDrilldownResponse profiles = usersApi.GetUserDSProfilesByEmail(organizationId, getProfilesOptions);
            var user = profiles.Users?[0];

            var organizationsApi = new OrganizationsApi(docusignClient);
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

            return organizationsApi.RedactIndividualUserData(organizationId, userRedactionRequest);
        }
    }
}
