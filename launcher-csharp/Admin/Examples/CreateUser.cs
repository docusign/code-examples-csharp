﻿// <copyright file="CreateUser.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Admin.Examples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.Admin.Model;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public class CreateUser
    {
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="organizationId">The DocuSign organization ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="firstName">The first name of a new user</param>
        /// <param name="lastName">The last name of a new user</param>
        /// <param name="userName">The username of a new user</param>
        /// <param name="email">The email of a new user</param>
        /// <param name="permissionProfileId">The permission profile ID that will be used for a new user</param>
        /// <param name="groupId">The group ID that will be used for a new user</param>
        /// <returns>The response of creating a new user</returns>
        public static NewUserResponse CreateNewUser(
            string accessToken,
            string basePath,
            Guid accountId,
            Guid? organizationId,
            string firstName,
            string lastName,
            string userName,
            string email,
            long permissionProfileId,
            long groupId)
        {
            // Construct your API headers
            //ds-snippet-start:Admin1Step2
            var apiClient = new DocuSign.Admin.Client.DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:Admin1Step2

            //ds-snippet-start:Admin1Step6
            var usersApi = new DocuSign.Admin.Api.UsersApi(apiClient);
            NewUserRequest newUserRequest = ConstructNewUserRequest(
                permissionProfileId,
                groupId,
                accountId,
                email,
                firstName,
                lastName,
                userName);

            return usersApi.CreateUser(organizationId, newUserRequest);
            //ds-snippet-end:Admin1Step6
        }

        /// <summary>
        /// Gets the DocuSign permission profiles and groups
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The tuple with DocuSign permission profiles and groups information</returns>
        public static (PermissionProfileInformation, GroupInformation) GetPermissionProfilesAndGroups(
            string accessToken, string basePath, string accountId)
        {
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            //ds-snippet-start:Admin1Step3
            var accountsApi = new AccountsApi(docuSignClient);
            var permissionProfiles = accountsApi.ListPermissions(accountId);
            //ds-snippet-end:Admin1Step3

            //ds-snippet-start:Admin1Step4
            var dsGroupsApi = new GroupsApi(docuSignClient);
            var groups = dsGroupsApi.ListGroups(accountId);
            //ds-snippet-end:Admin1Step4
            return (permissionProfiles, groups);
        }

        /// <summary>
        /// Constructs a request for creating a new user
        /// </summary>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="firstName">The first name of a new user</param>
        /// <param name="lastName">The last name of a new user</param>
        /// <param name="userName">The username of a new user</param>
        /// <param name="email">The email of a new user</param>
        /// <param name="permissionProfileId">The permission profile ID that will be used for a new user</param>
        /// <param name="groupId">The group ID that will be used for a new user</param>
        /// <returns>The request for creating a new user</returns>
        //ds-snippet-start:Admin1Step5
        private static NewUserRequest ConstructNewUserRequest(
            long permissionProfileId,
            long groupId,
            Guid accountId,
            string email,
            string firstName,
            string lastName,
            string userName)
        {
            return new NewUserRequest
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Accounts = new List<NewUserRequestAccountProperties>
                {
                    new NewUserRequestAccountProperties
                    {
                        Id = accountId,
                        PermissionProfile = new PermissionProfileRequest
                        {
                            Id = permissionProfileId,
                        },
                        Groups = new List<GroupRequest>
                        {
                            new GroupRequest
                            {
                                Id = groupId,
                            },
                        },
                    },
                },
                AutoActivateMemberships = true,
            };
        }

        //ds-snippet-end:Admin1Step5
    }
}
