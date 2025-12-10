// <copyright file="AssignFormToFormGroups.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
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
            ApiResponse<FormLibrarySummaryList> formLibraries = formLibrariesApi.GetFormLibrariesWithHttpInfo(accountId);

            formLibraries.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            formLibraries.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);

            ApiResponse<FormSummaryList> forms = null;
            if (formLibraries.Data.FormsLibrarySummaries.Any())
            {
                forms = formLibrariesApi.GetFormLibraryFormsWithHttpInfo(
                    accountId,
                    formLibraries.Data.FormsLibrarySummaries.First().FormsLibraryId);
            }

            forms.Headers.TryGetValue("X-RateLimit-Remaining", out remaining);
            forms.Headers.TryGetValue("X-RateLimit-Reset", out reset);

            resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);

            //ds-snippet-end:Rooms9Step3

            //ds-snippet-start:Rooms9Step4
            ApiResponse<FormGroupSummaryList> formGroups = formGroupsApi.GetFormGroupsWithHttpInfo(accountId);
            //ds-snippet-end:Rooms9Step4

            return (forms.Data, formGroups.Data);
        }

        /// <summary>
        /// Grants office access to a form group.
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI).</param>
        /// <param name="accessToken">Access Token for API call (OAuth).</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made.</param>
        /// <param name="formGroupId">The Id of the specified form group.</param>
        /// <param name="formToAssign">The form to be assigned to form group.</param>
        /// <returns>The response to assigning a form to form group.</returns>
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
            var response = formGroupsApi.AssignFormGroupFormWithHttpInfo(accountId, new Guid(formGroupId), formToAssign);

            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);

            return response.Data;
            //ds-snippet-end:Rooms9Step6
        }
    }
}
