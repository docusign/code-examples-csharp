// <copyright file="AssignFormToFormGroups.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Rooms.Examples
{
    using System;
    using System.Linq;
    using DocuSign.Rooms.Api;
    using DocuSign.Rooms.Client;
    using DocuSign.Rooms.Model;

    public class AssignFormToFormGroups
    {
        /// <summary>
        /// Gets the list of forms and form group.
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI).</param>
        /// <param name="accessToken">Access Token for API call (OAuth).</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made.</param>
        /// <returns>The tuple of forms and form group lists.</returns>
        public static (FormSummaryList forms, FormGroupSummaryList formGroups) GetFormsAndFormGroups(
            string basePath,
            string accessToken,
            string accountId)
        {
            // Construct your API headers
            //ds-snippet-start:Rooms9Step2
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            //ds-snippet-end:Rooms9Step2
            var formGroupsApi = new FormGroupsApi(apiClient);
            var formLibrariesApi = new FormLibrariesApi(apiClient);

            //ds-snippet-start:Rooms9Step3
            FormLibrarySummaryList formLibraries = formLibrariesApi.GetFormLibraries(accountId);

            FormSummaryList forms = new FormSummaryList();
            if (formLibraries.FormsLibrarySummaries.Any())
            {
                forms = formLibrariesApi.GetFormLibraryForms(
                    accountId,
                    formLibraries.FormsLibrarySummaries.First().FormsLibraryId);
            }

            //ds-snippet-end:Rooms9Step3

            //ds-snippet-start:Rooms9Step4
            FormGroupSummaryList formGroups = formGroupsApi.GetFormGroups(accountId);
            //ds-snippet-end:Rooms9Step4

            return (forms, formGroups);
        }

        /// <summary>
        /// Grants office access to a form group.
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI).</param>
        /// <param name="accessToken">Access Token for API call (OAuth).</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made.</param>
        /// <param name="formGroupId">The Id of the specified form group.</param>
        /// <param name="formToAssign">The form to be assigned to form group.</param>
        /// <returns>The form to be assigned to form group.</returns>
        public static FormGroupFormToAssign AssignForm(
            string basePath,
            string accessToken,
            string accountId,
            string formGroupId,
            FormGroupFormToAssign formToAssign)
        {
            // Construct your API headers
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var formGroupsApi = new FormGroupsApi(apiClient);

            // Call the Rooms API to assign form to form group
            //ds-snippet-start:Rooms9Step6
            return formGroupsApi.AssignFormGroupForm(accountId, new Guid(formGroupId), formToAssign);
            //ds-snippet-end:Rooms9Step6
        }
    }
}
