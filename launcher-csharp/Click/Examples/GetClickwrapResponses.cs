// <copyright file="GetClickwrapResponses.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Click.Examples
{
    using DocuSign.Click.Api;
    using DocuSign.Click.Client;
    using DocuSign.Click.Model;

    public class GetClickwrapResponses
    {
        /// <summary>
        /// Gets a clickwrap agreements
        /// </summary>
        /// <param name="clickwrapId">The Id of clickwrap</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The agreements of a given clickwrap</returns>
        public static ClickwrapAgreementsResponse GetAgreements(string clickwrapId, string basePath, string accessToken, string accountId)
        {
            //ds-snippet-start:Click5Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(docuSignClient);
            //ds-snippet-end:Click5Step2

            //ds-snippet-start:Click5Step3
            return clickAccountApi.GetClickwrapAgreements(accountId, clickwrapId);
            //ds-snippet-end:Click5Step3
        }
    }
}
