using System;
using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace eSignature.Examples
{
    public class ApplyBrandToEnvelope
    {
        /// <summary>
        /// Applies a brand to the envelope
        /// </summary>
        /// <param name="brandId">The brand ID</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="docPdf">String of bytes representing the document (pdf)</param>
        /// <returns>The summary of the envelopes</returns>
        public static EnvelopeSummary CreateEnvelopeWithBranding(string signerEmail, string signerName, string brandId, string accessToken, string basePath, string accountId, string status, string docPdf)
        {
            // Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Construct your request body
            EnvelopeDefinition env = CreateEnvelope(signerEmail, signerName, brandId, status, docPdf);

            // Call the eSignature REST API
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);

            return envelopesApi.CreateEnvelope(accountId, env);
        }

        private static EnvelopeDefinition CreateEnvelope(string signerEmail, string signerName, string brandId, string status, string docPdf)
        {          
            string docPdfBytes = Convert.ToBase64String(System.IO.File.ReadAllBytes(docPdf));
            // create the envelope definition
            EnvelopeDefinition env = new EnvelopeDefinition();
            env.EmailSubject = "Please sign this document set";

            // Create document objects, one per document
            Document doc = new Document
            {
                DocumentBase64 = docPdfBytes,
                Name = "Lorem Ipsum", // can be different from actual file name
                FileExtension = "pdf",
                DocumentId = "1"
            };
            // The order in the docs array determines the order in the envelope
            env.Documents = new List<Document> { doc };

            // create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            Signer signer1 = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                RecipientId = "1",
                RoutingOrder = "1"
            };


            // Create signHere fields (also known as tabs) on the documents,
            // We're using anchor (autoPlace) positioning
            //
            // The DocuSign platform searches throughout your envelope's
            // documents for matching anchor strings. So the
            // signHere2 tab will be used in both document 2 and 3 since they
            // use the same anchor string for their "signer 1" tabs.
            SignHere signHere1 = new SignHere
            {
                AnchorString = "**signature_1**",
                AnchorUnits = "pixels",
                AnchorYOffset = "10",
                AnchorXOffset = "20"
            };

            SignHere signHere2 = new SignHere
            {
                AnchorString = "/sn1/",
                AnchorUnits = "pixels",
                AnchorYOffset = "10",
                AnchorXOffset = "20"
            };

            // Tabs are set per recipient / signer
            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere1, signHere2 }
            };
            signer1.Tabs = signer1Tabs;

            // Add the recipients to the envelope object
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 }
            };
            env.Recipients = recipients;
            env.Status = status;

            //Set the brand id.
            env.BrandId = brandId;

            return env;
        }
    }
}
