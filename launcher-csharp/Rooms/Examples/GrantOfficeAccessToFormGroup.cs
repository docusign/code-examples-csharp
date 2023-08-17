// <copyright file="GrantOfficeAccessToFormGroup.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Rooms.Examples
{
    using System;
    using DocuSign.Rooms.Api;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Model;

    public class GrantOfficeAccessToFormGroup
    {
        /// <summary>
        /// Gets the list of offices and form group
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The tuple of office and form group lists</returns>
        public static (OfficeSummaryList offices, FormGroupSummaryList formGroups) GetOfficesAndFormGroups(
            string basePath,
            string accessToken,
            string accountId)
        {
            // Construct your API headers
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var officesApi = new OfficesApi(apiClient);
            var formGroupsApi = new FormGroupsApi(apiClient);

            // Call the Rooms API to get offices
            //ds-snippet-start:Rooms8Step3
            var offices = officesApi.GetOffices(accountId);
            //ds-snippet-end:Rooms8Step3

            // Call the Rooms API to get form groups
            //ds-snippet-start:Rooms8Step4
            var formGroups = formGroupsApi.GetFormGroups(accountId);
            //ds-snippet-end:Rooms8Step4

            return (offices, formGroups);
        }

        /// <summary>
        /// Grants office access to a form group
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="formGroupId">The Id of the specified form group</param>
        /// <param name="officeId">The Id of the specified office</param>
        /// <returns></returns>
        public static void GrantAccess(
            string basePath,
            string accessToken,
            string accountId,
            string formGroupId,
            int? officeId)
        {
            // Construct your API headers
            //ds-snippet-start:Rooms8Step2
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var formGroupsApi = new FormGroupsApi(apiClient);
            //ds-snippet-end:Rooms8Step2

            // Call the Rooms API to grant office access to a form group
            //ds-snippet-start:Rooms8Step5
            formGroupsApi.GrantOfficeAccessToFormGroup(accountId, new Guid(formGroupId), officeId);
            //ds-snippet-end:Rooms8Step5
        }
    }
}
