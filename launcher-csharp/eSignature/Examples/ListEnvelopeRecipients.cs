// <copyright file="ListEnvelopeRecipients.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class ListEnvelopeRecipients
    {
        /// <summary>
        /// Gets a list of all the recipients for a specific envelope
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="envelopeId">The required envelopeId</param>
        /// <returns>An object containing information about all the recipients in the envelope</returns>
        public static Recipients GetRecipients(string accessToken, string basePath, string accountId, string envelopeId)
        {
            //ds-snippet-start:eSign5Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            var results = envelopesApi.ListRecipientsWithHttpInfo(accountId, envelopeId);
            results.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            results.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);
            //ds-snippet-end:eSign5Step2

            return results.Data;
        }
    }
}
