using System;
using DocuSign.Rooms.Api;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Model;

namespace DocuSign.Rooms.Examples
{
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
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var officesApi = new OfficesApi(apiClient);
            var formGroupsApi = new FormGroupsApi(apiClient);

            // Call the Rooms API to get offices
            var offices = officesApi.GetOffices(accountId);

            // Call the Rooms API to get form groups
            var formGroups = formGroupsApi.GetFormGroups(accountId);

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
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", $"Bearer {accessToken}");
            var formGroupsApi = new FormGroupsApi(apiClient);

            // Call the Rooms API to grant office access to a form group
            formGroupsApi.GrantOfficeAccessToFormGroup(accountId, new Guid(formGroupId), officeId);
        }
    }
}
