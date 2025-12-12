// <copyright file="GetCustomFields.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class GetCustomFields
    {
        /// <summary>
        /// Get all the custom fields (text) from a specific envelope
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="envelopeId">The required envelopeId</param>
        /// <returns>Object with the custom fields data</returns>
        public static CustomFieldsEnvelope ListAllCustomFieldsForEnvelope(string accessToken, string basePath, string accountId, string envelopeId)
        {
            // Construct your API headers
            //ds-snippet-start:eSign18Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:eSign18Step2

            // Call the eSignature REST API
            //ds-snippet-start:eSign18Step3
            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            var listCustomFields = envelopesApi.ListCustomFieldsWithHttpInfo(accountId, envelopeId);
            listCustomFields.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            listCustomFields.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            return listCustomFields.Data;
            //ds-snippet-end:eSign18Step3
        }
    }
}
