// <copyright file="DeletePermission.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;

    public static class DeletePermission
    {
        /// <summary>
        /// Deletes a permission profile
        /// </summary>
        /// <param name="permissionProfileId">Permission profile ID</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        public static void DeletePermissionProfile(string permissionProfileId, string accessToken, string basePath, string accountId)
        {
            // Construct your API headers
            //ds-snippet-start:eSign27Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            AccountsApi accountsApi = new AccountsApi(docuSignClient);
            //ds-snippet-end:eSign27Step2

            // Call the eSignature REST API
            //ds-snippet-start:eSign27Step3
            accountsApi.DeletePermissionProfile(accountId, permissionProfileId);
            //ds-snippet-end:eSign27Step3
        }
    }
}
