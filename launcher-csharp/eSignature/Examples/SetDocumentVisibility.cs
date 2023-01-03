using System;
using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace eSignature.Examples
{
    public static class SetDocumentVisibility
    {
        /// <summary>
        /// Creates a new envelope and sends them to two signers with different document visibilities.
        /// </summary>
        /// <param name="signer1Email">Email address of the first signer</param>
        /// <param name="signer1Name">Full name of the first signer</param>
        /// <param name="signer2Email">Email address of the second signerr</param>
        /// <param name="signer2Name">Full name of the second signer</param>
        /// <param name="ccEmail">Email address of the cc</param>
        /// <param name="ccName">Full name of the cc</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="docPdf">Name of the document (pdf)</param>
        /// <param name="docDocx">Name of the document (docx)</param>
        /// <param name="docHTML">Name of the document (html)</param>
        /// <returns>The id of the created envelope</returns>
        public static string SendEnvelopeWithEnvelopeVisibility(string signer1Email, string signer1Name, string signer2Email, 
            string signer2Name, string ccEmail, string ccName, string accessToken, string basePath, string accountId,
            string docPdf, string docDocx, string docHTML) 
        {
            EnvelopeDefinition envelopeDefinition = PrepareEnvelope(signer1Email, signer1Name, signer2Email,
                    signer2Name, ccEmail, ccName, docPdf, docDocx, docHTML);
                  
            // Step 2 start
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            // Step 2 end

            // Step 4 start
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envelopeDefinition);
            // Step 4 end
            
            return envelopeSummary.EnvelopeId;
        }

        // Step 3 start
        private static EnvelopeDefinition PrepareEnvelope(string signer1Email, string signer1Name, string signer2Email, 
            string signer2Name, string ccEmail, string ccName, string docPdf, string docDocx, string docHTML)
        {
            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = "Please sign this document set",
                Documents = PrepareDocumentsForTemplate(docPdf, docDocx, docHTML)
            };

            Signer signer1 = PrepareSigner(signer1Email, signer1Name, "1", "1", new List<string> { "2", "3" }, 
                "**signature_1**", "pixels", "20", "10");
            Signer signer2 = PrepareSigner(signer2Email, signer2Name, "2", "1", new List<string> { "1" }, 
                "/sn1/", "pixels", "20", "10");

            CarbonCopy carbonCopy = new CarbonCopy
            {
                Email = ccEmail,
                Name = ccName,
                RecipientId = "3",
                RoutingOrder = "2"
            };

            envelopeDefinition.Recipients = new Recipients
            {
                CarbonCopies = new List<CarbonCopy> { carbonCopy },
                Signers = new List<Signer> { signer1, signer2 }
            };

            envelopeDefinition.Status = "sent";

            return envelopeDefinition;
        }

        private static Signer PrepareSigner(string signerEmail, string signerName, string recipientId, string routingOrder, 
            List<string> excludedDocuments, string tabsAnchorString, string tabsAnchorUnits, string tabsAnchorXOffset, 
            string tabsAnchorYOffset)
        {
            return new Signer
            {
                Email = signerEmail,
                Name = signerName,
                RecipientId = recipientId,
                RoutingOrder = routingOrder,
                ExcludedDocuments = excludedDocuments,
                Tabs = new Tabs
                {
                    SignHereTabs = new List<SignHere>
                    {
                        new SignHere
                        {
                            AnchorString = tabsAnchorString,
                            AnchorUnits = tabsAnchorUnits,
                            AnchorXOffset = tabsAnchorXOffset,
                            AnchorYOffset = tabsAnchorYOffset
                        }
                    }
                }
            };
        }

        private static List<Document> PrepareDocumentsForTemplate(string docPdf, string docDocx, string docHTML)
        {
            byte[] pdfFileContentInBytes = System.IO.File.ReadAllBytes(docPdf);
            byte[] docxFileContentInBytes = System.IO.File.ReadAllBytes(docDocx);
            byte[] htlmFileContentInBytes = System.IO.File.ReadAllBytes(docHTML);

            return new List<Document> 
            {
                new Document
                {
                    DocumentBase64 = Convert.ToBase64String(htlmFileContentInBytes),
                    Name = "Order acknowledgement",
                    FileExtension = "html",
                    DocumentId = "1"
                },
                new Document
                {
                    DocumentBase64 = Convert.ToBase64String(docxFileContentInBytes),
                    Name = "Battle Plan",
                    FileExtension = "docx",
                    DocumentId = "2"
                },
                new Document
                {
                    DocumentBase64 = Convert.ToBase64String(pdfFileContentInBytes),
                    Name = "Lorem Ipsum",
                    FileExtension = "pdf",
                    DocumentId = "3"
                }
            };
        }
        // Step 3 end
    }
}
