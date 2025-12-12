// <copyright file="GetEnvelopeInformation.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class GetEnvelopeInformation
    {
        /// <summary>
        /// Gets the envelope's information for the specified envelope
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="envelopeId">The envelopeId for which you wish to get information</param>
        /// <returns>Object containing envelope information</returns>
        public static Envelope GetEnvelope(string accessToken, string basePath, string accountId, string envelopeId)
        {
            //ds-snippet-start:eSign4Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            var getEnvelopeReponse = envelopesApi.GetEnvelopeWithHttpInfo(accountId, envelopeId);
            getEnvelopeReponse.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            getEnvelopeReponse.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            return getEnvelopeReponse.Data;
            //ds-snippet-end:eSign4Step2
        }
    }
}
