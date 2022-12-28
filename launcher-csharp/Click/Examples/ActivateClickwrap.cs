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
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(docuSignClient);

            var clickwrapRequest = BuildUpdateClickwrapVersionRequest();

            return clickAccountApi.UpdateClickwrapVersion(accountId, clickwrapId, clickwrapVersion, clickwrapRequest);
        }

        private static ClickwrapRequest BuildUpdateClickwrapVersionRequest()
        {
            var clickwrapRequest = new ClickwrapRequest
            {
                Status = "active",
            };

            return clickwrapRequest;
        }

        /// <summary>
        /// Gets a list of inactive clickwraps
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="accessToken"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static ClickwrapVersionsResponse GetInactiveClickwraps(string basePath, string accessToken, string accountId)
        {
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var clickAccountApi = new AccountsApi(docuSignClient);
            var options = new AccountsApi.GetClickwrapsOptions();
            options.status = "inactive";

            return clickAccountApi.GetClickwraps(accountId, options);
        }
    }
}
