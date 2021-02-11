using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using System.Collections.Generic;


namespace eSignature.Examples
{
    public static class ListEnvelopeDocuments
    {
        public class EnvelopeDocItem
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string DocumentId { get; set; }
        }

        public class EnvelopeDocuments
        {
            public string EnvelopeId { get; set; }
            public List<EnvelopeDocItem> Documents { get; set; }
        }

        /// <summary>
        /// Gets a list of all the documents for a specific envelope
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="envelopeId">The required envelopeId</param>
        /// <returns>An object containing information about all the documents in the envelopes</returns>
        public static EnvelopeDocuments GetDocuments(string accessToken, string basePath, string accountId, string envelopeId)
        {
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeDocumentsResult results = envelopesApi.ListDocuments(accountId, envelopeId);

            List<EnvelopeDocItem> envelopeDocItems = new List<EnvelopeDocItem>
            {
                new EnvelopeDocItem { Name = "Combined", Type = "content", DocumentId = "combined" },
                new EnvelopeDocItem { Name = "Zip archive", Type = "zip", DocumentId = "archive" }
            };

            foreach (EnvelopeDocument doc in results.EnvelopeDocuments)
            {
                envelopeDocItems.Add(new EnvelopeDocItem
                {
                    DocumentId = doc.DocumentId,
                    Name = doc.DocumentId == "certificate" ? "Certificate of completion" : doc.Name,
                    Type = doc.Type
                });
            }

            EnvelopeDocuments envelopeDocuments = new EnvelopeDocuments
            {
                EnvelopeId = envelopeId,
                Documents = envelopeDocItems
            };

            return envelopeDocuments;
        }
    }
}
