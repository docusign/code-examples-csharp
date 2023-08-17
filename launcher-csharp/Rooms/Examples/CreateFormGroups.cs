// <copyright file="CreateFormGroups.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Rooms.Examples
{
    using DocuSign.Rooms.Api;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Model;

    public class CreateFormGroups
    {
        /// <summary>
        /// Creates form group
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="groupName">The name of new group</param>
        /// <returns>The new form group</returns>
        public static FormGroup CreateGroup(
            string basePath,
            string accessToken,
            string accountId,
            string groupName)
        {
            // Construct your API headers
            //ds-snippet-start:Rooms7Step2
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var formGroupsApi = new FormGroupsApi(apiClient);
            //ds-snippet-end:Rooms7Step2

            //ds-snippet-start:Rooms7Step3
            var formGroupForCreate = new FormGroupForCreate(groupName);
            //ds-snippet-end:Rooms7Step3

            // Call the Rooms API to create form group
            //ds-snippet-start:Rooms7Step4
            return formGroupsApi.CreateFormGroup(accountId, formGroupForCreate);
            //ds-snippet-end:Rooms7Step4
        }
    }
}
