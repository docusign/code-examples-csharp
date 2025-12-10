// <copyright file="GetClickwrapResponses.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.Click.Examples
{
    using System;
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
            var response = clickAccountApi.GetClickwrapAgreementsWithHttpInfo(accountId, clickwrapId);

            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);

            return response.Data;
            //ds-snippet-end:Click5Step3
        }
    }
}
