using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;

namespace eSignature.Examples
{
    public static class GetDocumentFromEnvelope
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
        /// Download a specific document from an envelope
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="envelopeId">The required envelopeId</param>
        /// <param name="documents">Object containing all documents information</param>
        /// <param name="documentId">The required document ID</param>
        /// <returns>Stream containing the document, mimeType for this document and the document name</returns>
        public static (Stream, string, string) DownloadDocument(string accessToken, string basePath, string accountId, string envelopeId, List<EnvelopeDocItem> documents, string documentId)
        {
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);

            // Step 1. EnvelopeDocuments::get.
            // Exceptions will be caught by the calling function
            Stream results = envelopesApi.GetDocument(accountId, envelopeId, documentId);

            // Step 2. Look up the document from the list of documents 
            EnvelopeDocItem docItem = documents.FirstOrDefault(d => documentId.Equals(d.DocumentId));
            // Process results. Determine the file name and mimetype
            string docName = docItem.Name;
            bool hasPDFsuffix = docName.ToUpper().EndsWith(".PDF");
            bool pdfFile = hasPDFsuffix;
            // Add .pdf if it's a content or summary doc and doesn't already end in .pdf
            string docType = docItem.Type;
            if (("content".Equals(docType) || "summary".Equals(docType)) && !hasPDFsuffix)
            {
                docName += ".pdf";
                pdfFile = true;
            }
            // Add .zip as appropriate
            if ("zip".Equals(docType))
            {
                docName += ".zip";
            }
            // Return the file information
            // See https://stackoverflow.com/a/30625085/64904
            string mimetype;
            if (pdfFile)
            {
                mimetype = "application/pdf";
            }
            else if ("zip".Equals(docType))
            {
                mimetype = "application/zip";
            }
            else
            {
                mimetype = "application/octet-stream";
            }

            return (results, mimetype, docName);
        }
    }
}
