// <copyright file="GetEnvelopeInformation.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
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
            return envelopesApi.GetEnvelope(accountId, envelopeId);
            //ds-snippet-end:eSign4Step2
        }
    }
}
