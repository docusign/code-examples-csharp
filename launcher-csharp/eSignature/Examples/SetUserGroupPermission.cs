// <copyright file="SetUserGroupPermission.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System.Collections.Generic;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class SetUserGroupPermission
    {
        /// <summary>
        /// Sets permission to the user group
        /// </summary>
        /// <param name="permissionProfileId">Permission profile id</param>
        /// <param name="userGroupId">User group id</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>A group information</returns>
        public static GroupInformation GetGroupInformation(string permissionProfileId, string userGroupId, string accessToken, string basePath, string accountId)
        {
            // Construct your API headers
            //ds-snippet-start:eSign25Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:eSign25Step2
            var groupsApi = new GroupsApi(docuSignClient);

            // Construct your request body
            //ds-snippet-start:eSign25Step3
            var editedGroup = new Group
            {
                GroupId = userGroupId,
                PermissionProfileId = permissionProfileId,
            };
            var requestBody = new GroupInformation { Groups = new List<Group> { editedGroup } };
            //ds-snippet-end:eSign25Step3

            // Call the eSignature REST API
            //ds-snippet-start:eSign25Step4
            return groupsApi.UpdateGroups(accountId, requestBody);
            //ds-snippet-end:eSign25Step4
        }
    }
}
