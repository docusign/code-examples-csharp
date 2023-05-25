// <copyright file="RetrieveClickwraps.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Click.Examples
{
    using DocuSign.Click.Api;
    using DocuSign.Click.Client;
    using DocuSign.Click.Model;

    public class RetrieveClickwraps
    {
        /// <summary>
        /// Gets a list of all clickwraps
        /// </summary>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The list of all clickwraps</returns>
        public static ClickwrapVersionsResponse GetClickwraps(string basePath, string accessToken, string accountId)
        {
            //ds-snippet-start:Click4Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:Click4Step2
            //ds-snippet-start:Click4Step3
            var clickAccountApi = new AccountsApi(docuSignClient);

            return clickAccountApi.GetClickwraps(accountId);
            //ds-snippet-end:Click4Step3
        }
    }
}
