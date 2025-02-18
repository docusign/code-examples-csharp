// <copyright file="SendWithThirdPartyNotary.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class SendWithThirdPartyNotary
    {
        public static string SendWithNotary(string signerEmail, string signerName, string accessToken, string basePath, string accountId, string envStatus)
        {
            //ds-snippet-start:Notary4Step2
            EnvelopeDefinition env = MakeEnvelope(signerEmail, signerName, envStatus);
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:Notary4Step2

            //ds-snippet-start:Notary4Step4
            var envelopesApi = new EnvelopesApi(docuSignClient);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, env);
            return results.EnvelopeId;
            //ds-snippet-end:Notary4Step4
        }

        //ds-snippet-start:Notary4Step3
        private static EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName, string envStatus)
        {
            var env = new EnvelopeDefinition
            {
                EmailSubject = "Please sign this document set",
            };

            env.Documents = GetDocuments(signerEmail, signerName);

            var signers = GetSigners(signerEmail, signerName);

            var notaryRecipients = GetNotaryRecipients();

            // Add the recipients to the envelope object
            var recipients = new Recipients
            {
                Signers = signers,
                Notaries = notaryRecipients,
            };
            env.Recipients = recipients;

            // Request that the envelope be sent by setting |status| to "sent".
            // To request that the envelope be created as a draft, set to "created"
            env.Status = envStatus;

            return env;
        }

        private static List<Document> GetDocuments(string signerEmail, string signerName)
        {
            // Create document objects, one per document
            var doc1 = new Document();
            var b64 = Convert.ToBase64String(GetDocumentExample(signerEmail, signerName));
            doc1.DocumentBase64 = b64;
            doc1.Name = "Order acknowledgement"; // can be different from actual file name
            doc1.FileExtension = "html"; // Source data format. Signed docs are always pdf.
            doc1.DocumentId = "1"; // a label used to reference the doc

            return new List<Document> { doc1 };
        }

        private static byte[] GetDocumentExample(string signerEmail, string signerName)
        {
            return Encoding.UTF8.GetBytes(" <!DOCTYPE html>\n"
                                          + "    <html>\n"
                                          + "        <head>\n"
                                          + "          <meta charset=\"UTF-8\">\n"
                                          + "        </head>\n"
                                          + "        <body style=\"font-family:sans-serif;margin-left:2em;\">\n"
                                          + "        <h1 style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\n"
                                          + "            color: darkblue;margin-bottom: 0;\">World Wide Corp</h1>\n"
                                          + "        <h2 style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\n"
                                          + "          margin-top: 0px;margin-bottom: 3.5em;font-size: 1em;\n"
                                          + "          color: darkblue;\">Order Processing Division</h2>\n"
                                          + "        <h4>Ordered by "
                                          + signerName
                                          + "</h4>\n"
                                          + "        <p style=\"margin-top:0em; margin-bottom:0em;\">Email: "
                                          + signerEmail
                                          + "</p>\n"
                                          + "        <p style=\"margin-top:3em;\">\n"
                                          + "  Candy bonbon pastry jujubes lollipop wafer biscuit biscuit. Topping brownie sesame snaps sweet roll pie. Croissant danish biscuit soufflé caramels jujubes jelly. Dragée danish caramels lemon drops dragée. Gummi bears cupcake biscuit tiramisu sugar plum pastry. Dragée gummies applicake pudding liquorice. Donut jujubes oat cake jelly-o. Dessert bear claw chocolate cake gummies lollipop sugar plum ice cream gummies cheesecake.\n"
                                          + "        </p>\n"
                                          + "        <!-- Note the anchor tag for the signature field is in white. -->\n"
                                          + "        <h3 style=\"margin-top:3em;\">Agreed: <span style=\"color:white;\">**signature_1**/</span></h3>\n"
                                          + "        </body>\n"
                                          + "    </html>");
        }

        private static List<Signer> GetSigners(string signerEmail, string signerName)
        {
            // create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            var signer1 = new Signer
            {
                ClientUserId = "1000",
                Email = signerEmail,
                Name = signerName,
                RecipientId = "2",
                RoutingOrder = "1",
                NotaryId = "1",
            };

            // Create signHere fields (also known as tabs) on the documents,
            // We're using anchor (autoPlace) positioning
            //
            // The DocuSign platform searches throughout your envelope's
            // documents for matching anchor strings. So the
            // signHere2 tab will be used in both document 2 and 3 since they
            // use the same anchor string for their "signer 1" tabs.
            var signHere1 = new SignHere
            {
                DocumentId = "1",
                XPosition = "200",
                YPosition = "235",
                PageNumber = "1",
            };

            var signHere2 = new SignHere
            {
                StampType = "stamp",
                DocumentId = "1",
                XPosition = "200",
                YPosition = "150",
                PageNumber = "1",
            };

            // Tabs are set per recipient
            var signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere1, signHere2 },
            };
            signer1.Tabs = signer1Tabs;

            return new List<Signer> { signer1 };
        }

        private static List<NotaryRecipient> GetNotaryRecipients()
        {
            var notarySealTabs = new NotarySeal
            {
                XPosition = "300",
                YPosition = "235",
                DocumentId = "1",
                PageNumber = "1",
            };

            var notarySignHere = new SignHere
            {
                XPosition = "300",
                YPosition = "150",
                DocumentId = "1",
                PageNumber = "1",
            };

            var notaryTabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { notarySignHere },
                NotarySealTabs = new List<NotarySeal> { notarySealTabs },
            };

            var notaryRecipient = new List<NotaryRecipient>
            {
                new NotaryRecipient
                {
                    Email = string.Empty,
                    Name = "Notary",
                    RecipientId = "1",
                    RoutingOrder = "1",
                    Tabs = notaryTabs,
                    NotaryType = "remote",
                    NotarySourceType = "thirdparty",
                    NotaryThirdPartyPartner = "onenotary",
                    RecipientSignatureProviders = new List<RecipientSignatureProvider>
                    {
                        new RecipientSignatureProvider
                        {
                            SealDocumentsWithTabsOnly = "false",
                            SignatureProviderName = "ds_authority_idv",
                            SignatureProviderOptions = new RecipientSignatureProviderOptions(),
                        },
                    },
                },
            };

            return notaryRecipient;
        }

        //ds-snippet-end:Notary4Step3
    }
}
