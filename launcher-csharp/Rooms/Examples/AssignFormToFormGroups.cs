using System;
using System.Collections.Generic;
using System.Linq;
using DocuSign.Rooms.Api;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Model;

namespace DocuSign.Rooms.Examples
{
    public class AssignFormToFormGroups
    {
        /// <summary>
        /// Gets the list of forms and form group
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The tuple of forms and form group lists</returns>
        public static (FormSummaryList forms, FormGroupSummaryList formGroups) GetFormsAndFormGroups(
            string basePath,
            string accessToken,
            string accountId)
        {
            // Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var formGroupsApi = new FormGroupsApi(apiClient);
            var formLibrariesApi = new FormLibrariesApi(apiClient);

            FormLibrarySummaryList formLibraries = formLibrariesApi.GetFormLibraries(accountId);

            FormSummaryList forms = new FormSummaryList();
            if (formLibraries.FormsLibrarySummaries.Any())
            {
                forms = formLibrariesApi.GetFormLibraryForms(
                    accountId,
                    formLibraries.FormsLibrarySummaries.First().FormsLibraryId);
            }

            FormGroupSummaryList formGroups = formGroupsApi.GetFormGroups(accountId);

            return (forms, formGroups);
        }

        /// <summary>
        /// Grants office access to a form group
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="formGroupId">The Id of the specified form group</param>
        /// <param name="formToAssign">The form to be assigned to form group</param>
        /// <returns>The form to be assigned to form group</returns>
        public static FormGroupFormToAssign AssignForm(
            string basePath,
            string accessToken,
            string accountId,
            string formGroupId,
            FormGroupFormToAssign formToAssign)
        {
            // Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var formGroupsApi = new FormGroupsApi(apiClient);

            // Call the Rooms API to assign form to form group
            return formGroupsApi.AssignFormGroupForm(accountId, new Guid(formGroupId), formToAssign);
        }
    }
}
