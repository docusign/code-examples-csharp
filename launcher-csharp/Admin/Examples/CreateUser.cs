using DocuSign.Admin.Model;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using System;
using System.Collections.Generic;

namespace DocuSign.CodeExamples.Admin.Examples
{
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
            string accessToken, string basePath, Guid accountId, Guid? organizationId, string firstName, string lastName,
            string userName, string email, long permissionProfileId, long groupId)
        {
            // Construct your API headers
            // Step 2 start
            var apiClient = new DocuSign.Admin.Client.ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            // Step 2 end

            // Step 4 start
            var usersApi = new DocuSign.Admin.Api.UsersApi(apiClient);
            NewUserRequest newUserRequest = ConstructNewUserRequest(permissionProfileId, groupId, accountId, email,
                firstName, lastName, userName);

            return usersApi.CreateUser(organizationId, newUserRequest);
            // Step 4 end
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
            var apiClient = new eSign.Client.ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var accountsApi = new AccountsApi(apiClient);
            var permissionProfiles = accountsApi.ListPermissions(accountId);

            var dsGroupsApi = new GroupsApi(apiClient);
            var groups = dsGroupsApi.ListGroups(accountId);

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
        private static NewUserRequest ConstructNewUserRequest(
            long permissionProfileId, long groupId, Guid accountId, string email, string firstName, string lastName,
            string userName)
        {
            return new NewUserRequest
            {
                // Step 3 start
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
                            Id = permissionProfileId
                        },
                        Groups = new List<GroupRequest>
                        {
                            new GroupRequest
                            {
                                Id = groupId
                            }
                        }
                    }
                },
                AutoActivateMemberships = true
                // Step 3 end
            };
        }
    }
}
