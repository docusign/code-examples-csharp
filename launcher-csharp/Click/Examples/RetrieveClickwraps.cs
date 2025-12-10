// <copyright file="RetrieveClickwraps.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.Click.Examples
{
    using System;
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

            var response = clickAccountApi.GetClickwrapsWithHttpInfo(accountId);

            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);

            return response.Data;
            //ds-snippet-end:Click4Step3
        }
    }
}
