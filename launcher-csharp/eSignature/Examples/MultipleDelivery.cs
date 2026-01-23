// <copyright file="MultipleDelivery.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public class MultipleDelivery
    {
        /// <summary>
        /// Created an envelope.
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth).</param>
        /// <param name="basePath">BasePath for API calls (URI).</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="envelopeDefinition">Definition of the prepared envelope.</param>
        /// <returns>EnvelopeId for the new envelope.</returns>
        public static string SendRequestByMultipleChannels(
            string accessToken,
            string basePath,
            string accountId,
            EnvelopeDefinition envelopeDefinition)
        {
            var docusignClient = new DocuSignClient(basePath);
            docusignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(docusignClient);

            var results = envelopesApi.CreateEnvelopeWithHttpInfo(accountId, envelopeDefinition);
            results.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            results.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            if (reset != null && remaining != null)
            {
                DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;
                Console.WriteLine("API calls remaining: " + remaining);
                Console.WriteLine("Next Reset: " + resetDate);
            }

            return results.Data.EnvelopeId;
        }

        /// <summary>
        /// Prepares an envelope definition would include two documents and add a signer and cc recipients to be notified via multiple delivery channels (Email and SMS or WhatsApp).
        /// </summary>
        /// <param name="signerName">Full name of the signer.</param>
        /// <param name="signerEmail">Email of the signer.</param>
        /// <param name="signerCountryCode">Country code of the signer.</param>
        /// <param name="signerPhoneNumber">Phone number of the signer.</param>
        /// <param name="ccName">Name of the cc recipient.</param>
        /// <param name="ccEmail">Email of the cc recipient.</param>
        /// <param name="ccCountryCode">Country code of the cc recipient.</param>
        /// <param name="ccPhoneNumber">Phone number of the cc recipient.</param>
        /// <param name="docPdf">String of bytes representing the document (pdf).</param>
        /// <param name="docDocx">String of bytes representing the Word document (docx).</param>
        /// <param name="envStatus">Status to set the envelope to.</param>
        /// <param name="deliveryMethod">SMS or WhatsApp</param>
        /// <returns>EnvelopeId for the new envelope.</returns>
        public static EnvelopeDefinition MakeEnvelope(
            string signerName,
            string signerEmail,
            string signerCountryCode,
            string signerPhoneNumber,
            string ccName,
            string ccEmail,
            string ccCountryCode,
            string ccPhoneNumber,
            string docDocx,
            string docPdf,
            string envStatus,
            string deliveryMethod)
        {
            string doc2DocxBytes = Convert.ToBase64String(System.IO.File.ReadAllBytes(docDocx));
            string doc3PdfBytes = Convert.ToBase64String(System.IO.File.ReadAllBytes(docPdf));
            string b64 = Convert.ToBase64String(Document1(signerPhoneNumber, signerName, ccPhoneNumber, ccName));

            Document document1 = new Document
            {
                DocumentBase64 = b64,
                Name = "Order acknowledgement",
                FileExtension = "html",
                DocumentId = "1",
            };

            Document document2 = new Document
            {
                DocumentBase64 = doc2DocxBytes,
                Name = "Battle Plan",
                FileExtension = "docx",
                DocumentId = "2",
            };

            Document document3 = new Document
            {
                DocumentBase64 = doc3PdfBytes,
                Name = "Lorem Ipsum",
                FileExtension = "pdf",
                DocumentId = "3",
            };

            Signer signer = new Signer
            {
                Name = signerName,
                RecipientId = "1",
                RoutingOrder = "1",
                Email = signerEmail,
                DeliveryMethod = "Email",
                AdditionalNotifications = new List<RecipientAdditionalNotification>
                {
                    new RecipientAdditionalNotification
                    {
                        SecondaryDeliveryMethod = deliveryMethod,
                        PhoneNumber = new RecipientPhoneNumber
                        {
                            CountryCode = signerCountryCode,
                            Number = signerPhoneNumber,
                        },
                    },
                },
            };

            Tabs signerTabs = new Tabs
            {
                SignHereTabs = new List<SignHere>
                {
                    new SignHere
                    {
                        AnchorString = "**signature_1**",
                        AnchorUnits = "pixels",
                        AnchorYOffset = "10",
                        AnchorXOffset = "20",
                    },
                    new SignHere
                    {
                        AnchorString = "/sn1/",
                        AnchorUnits = "pixels",
                        AnchorYOffset = "10",
                        AnchorXOffset = "20",
                    },
                },
            };
            signer.Tabs = signerTabs;

            CarbonCopy cc = new CarbonCopy
            {
                Name = ccName,
                RecipientId = "2",
                RoutingOrder = "2",
                Email = ccEmail,
                DeliveryMethod = "Email",
                AdditionalNotifications = new List<RecipientAdditionalNotification>
                {
                    new RecipientAdditionalNotification
                    {
                        SecondaryDeliveryMethod = deliveryMethod,
                        PhoneNumber = new RecipientPhoneNumber
                        {
                            CountryCode = ccCountryCode,
                            Number = ccPhoneNumber,
                        },
                    },
                },
            };

            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer },
                CarbonCopies = new List<CarbonCopy> { cc },
            };

            EnvelopeDefinition env = new EnvelopeDefinition
            {
                EmailSubject = "Please sign this document set",
                Documents = new List<Document> { document1, document2, document3 },
                Recipients = recipients,
                Status = envStatus,
            };

            return env;
        }

        private static byte[] Document1(string signerPhone, string signerName, string ccPhone, string ccName)
        {
            return Encoding.UTF8.GetBytes(
            " <!DOCTYPE html>\n" +
                "    <html>\n" +
                "        <head>\n" +
                "          <meta charset=\"UTF-8\">\n" +
                "        </head>\n" +
                "        <body style=\"font-family:sans-serif;margin-left:2em;\">\n" +
                "        <h1 style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\n" +
                "            color: darkblue;margin-bottom: 0;\">World Wide Corp</h1>\n" +
                "        <h2 style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\n" +
                "          margin-top: 0px;margin-bottom: 3.5em;font-size: 1em;\n" +
                "          color: darkblue;\">Order Processing Division</h2>\n" +
                "        <h4>Ordered by " + signerName + "</h4>\n" +
                "        <p style=\"margin-top:0em; margin-bottom:0em;\">Phone Number: " + signerPhone + "</p>\n" +
                "        <p style=\"margin-top:0em; margin-bottom:0em;\">Copy to: " + ccName + ", " + ccPhone + "</p>\n" +
                "        <p style=\"margin-top:3em;\">\n" +
                "  Candy bonbon pastry jujubes lollipop wafer biscuit biscuit. Topping brownie sesame snaps sweet roll pie. Croissant danish biscuit soufflé caramels jujubes jelly. Dragée danish caramels lemon drops dragée. Gummi bears cupcake biscuit tiramisu sugar plum pastry. Dragée gummies applicake pudding liquorice. Donut jujubes oat cake jelly-o. Dessert bear claw chocolate cake gummies lollipop sugar plum ice cream gummies cheesecake.\n" +
                "        </p>\n" +
                "        <!-- Note the anchor tag for the signature field is in white. -->\n" +
                "        <h3 style=\"margin-top:3em;\">Agreed: <span style=\"color:white;\">**signature_1**/</span></h3>\n" +
                "        </body>\n" +
                "    </html>");
        }
    }
}
