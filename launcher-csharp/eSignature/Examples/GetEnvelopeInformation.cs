using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace eSignature.Examples
{
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
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            return envelopesApi.GetEnvelope(accountId, envelopeId);
        }
    }
}
