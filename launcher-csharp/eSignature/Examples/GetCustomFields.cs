// <copyright file="GetCustomFields.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
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
            return envelopesApi.ListCustomFields(accountId, envelopeId);
            //ds-snippet-end:eSign18Step3
        }
    }
}
