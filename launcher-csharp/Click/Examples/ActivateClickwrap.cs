// <copyright file="ActivateClickwrap.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.Click.Examples
{
    using DocuSign.Click.Api;
    using DocuSign.Click.Client;
    using DocuSign.Click.Model;

    public class ActivateClickwrap
    {
        /// <summary>
        /// Activates a newly created clickwrap
        /// </summary>
        /// <param name="clickwrapId">The Id of clickwrap</param>
        /// <param name="clickwrapVersion">The version of clickwrap</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The summary response of the activated clickwrap</returns>
        public static ClickwrapVersionSummaryResponse Update(string clickwrapId, string clickwrapVersion, string basePath, string accessToken, string accountId)
        {
            //ds-snippet-start:Click2Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(docuSignClient);
            //ds-snippet-end:Click2Step2

            //ds-snippet-start:Click2Step3
            var clickwrapRequest = BuildUpdateClickwrapVersionRequest();
            //ds-snippet-end:Click2Step3

            //ds-snippet-start:Click2Step4
            return clickAccountApi.UpdateClickwrapVersion(accountId, clickwrapId, clickwrapVersion, clickwrapRequest);
            //ds-snippet-end:Click2Step4
        }

        //ds-snippet-start:Click2Step3
        public static ClickwrapRequest BuildUpdateClickwrapVersionRequest()
        {
            var clickwrapRequest = new ClickwrapRequest
            {
                Status = "active",
            };

            return clickwrapRequest;
        }

        //ds-snippet-end:Click2Step3

        /// <summary>
        /// Gets a list of clickwraps by status
        /// </summary>
        /// <param name="basePath">Base path</param>
        /// <param name="accessToken">Access token</param>
        /// <param name="accountId">Account id</param>
        /// <param name="status">Status</param>
        /// <returns>ClickwrapVersionsResponse</returns>
        public static ClickwrapVersionsResponse GetClickwrapsByStatus(string basePath, string accessToken, string accountId, string status)
        {
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(docuSignClient);
            var options = new AccountsApi.GetClickwrapsOptions();
            options.status = status;

            return clickAccountApi.GetClickwraps(accountId, options);
        }
    }
}
