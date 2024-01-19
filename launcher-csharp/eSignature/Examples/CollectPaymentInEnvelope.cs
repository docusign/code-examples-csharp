// <copyright file="CollectPaymentInEnvelope.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class CollectPaymentInEnvelope
    {
        /// <summary>
        /// Creates an envelope that would include payment processing to collect payment from recipient
        /// </summary>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="ccEmail">Email address for the cc recipient</param>
        /// <param name="ccName">Name of the cc recipient</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="envStatus">Status to set the envelope to</param>
        /// <param name="paymentDetails">Object containing all the necassary information to process payments</param>
        /// <returns>EnvelopeId for the new envelope</returns>
        public static string CreateEnvelopeWithPayment(
            string signerEmail,
            string signerName,
            string ccEmail,
            string ccName,
            string accessToken,
            string basePath,
            string accountId,
            string envStatus,
            string gatawayAccountId,
            string gatewayName,
            string gatewayDisplayName)
        {
            // Data for this method
            // signerEmail
            // signerName
            // ccEmail
            // ccName
            // accessToken
            // basePath
            // accountId
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);

            // Step 1. Make the envelope request body
            //ds-snippet-start:eSign14Step3
            EnvelopeDefinition envelope = MakeEnvelope(signerEmail, signerName, ccEmail, ccName, envStatus, gatawayAccountId, gatewayName, gatewayDisplayName);
            //ds-snippet-end:eSign14Step3

            // Step 2. call Envelopes::create API method
            // Exceptions will be caught by the calling function
            //ds-snippet-start:eSign14Step4
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, envelope);
            //ds-snippet-end:eSign14Step4
            return results.EnvelopeId;
        }

        //ds-snippet-start:eSign14Step3
        private static EnvelopeDefinition MakeEnvelope(
            string signerEmail,
            string signerName,
            string ccEmail,
            string ccName,
            string envStatus,
            string gatewayAccountId,
            string gatewayName,
            string gatewayDisplayName)
        {
            // Data for this method
            // signerEmail
            // signerName
            // ccEmail
            // ccName
            // envStatus

            // document 1 (html) has multiple tags:
            // /l1q/ and /l2q/ -- quantities: drop down
            // /l1e/ and /l2e/ -- extended: payment lines
            // /l3t/ -- total -- formula
            //
            // The envelope has two recipients.
            // recipient 1 - signer
            // recipient 2 - cc
            // The envelope will be sent first to the signer.
            // After it is signed, a copy is sent to the cc person.

            ///////////////////////////////////////////////////////////////////
            //                                                               //
            // NOTA BENA: This method programmatically constructs the        //
            //            order form. For many use cases, it would be        //
            //            better to create the order form as a template      //
            //            using the DocuSign web tool as a WYSIWYG           //
            //            form designer.                                     //
            //                                                               //
            ///////////////////////////////////////////////////////////////////

            // Order form constants
            int l1Price = 5;
            int l2Price = 150;
            int currencyMultiplier = 100;
            string l1Name = "Harmonica";
            string l1Description = $"${l1Price} each",
                 l2Name = "Xylophone";
            string l2Description = $"${l2Price} each";

            // Payment line items
            PaymentLineItem paymentLineIteml1 = new PaymentLineItem
            {
                Name = l1Name,
                Description = l1Description,
                AmountReference = "l1e",
            },
            paymentLineIteml2 = new PaymentLineItem
            {
                Name = l2Name,
                Description = l2Description,
                AmountReference = "l2e",
            };
            PaymentDetails paymentDetails = new PaymentDetails
            {
                GatewayAccountId = gatewayAccountId,
                CurrencyCode = "USD",
                GatewayName = gatewayName,
                GatewayDisplayName = gatewayDisplayName,
                LineItems = new List<PaymentLineItem> { paymentLineIteml1, paymentLineIteml2 },
            };

            // read file from a local directory
            // The read could raise an exception if the file is not available!
            string doc1Html1 = System.IO.File.ReadAllText("order_form.html");

            // Substitute values into the HTML
            // Substitute for: {signerName}, {signerEmail}, {ccName}, {ccEmail}
            var doc1Html2 = doc1Html1.Replace("{signerName}", signerName)
                    .Replace("{signerEmail}", signerEmail)
                    .Replace("{ccName}", ccName)
                    .Replace("{ccEmail}", ccEmail);

            // create the envelope definition
            EnvelopeDefinition env = new EnvelopeDefinition
            {
                EmailSubject = "Please complete your order",
            };

            // add the documents
            string doc1B64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(doc1Html2));
            Document doc1 = new Document
            {
                DocumentBase64 = doc1B64,
                Name = "Order form", // can be different from actual file name
                FileExtension = "html", // Source data format. Signed docs are always pdf.
                DocumentId = "1", // a label used to reference the doc
            };
            env.Documents = new List<Document> { doc1 };

            // create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            Signer signer1 = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                RecipientId = "1",
                RoutingOrder = "1",
            };

            // routingOrder (lower means earlier) determines the order of deliveries
            // to the recipients. Parallel routing order is supported by using the
            // same integer as the order for two or more recipients.

            // create a cc recipient to receive a copy of the documents, identified by name and email
            // We're setting the parameters via setters
            CarbonCopy cc1 = new CarbonCopy
            {
                Email = ccEmail,
                Name = ccName,
                RoutingOrder = "2",
                RecipientId = "2",
            };

            // Create signHere fields (also known as tabs) on the documents,
            // We're using anchor (autoPlace) positioning
            SignHere signHere1 = new SignHere
            {
                AnchorString = "/sn1/",
                AnchorYOffset = "10",
                AnchorUnits = "pixels",
                AnchorXOffset = "20",
            };
            ListItem listItem0 = new ListItem { Text = "none", Value = "0" },
                   listItem1 = new ListItem { Text = "1", Value = "1" },
                   listItem2 = new ListItem { Text = "2", Value = "2" },
                   listItem3 = new ListItem { Text = "3", Value = "3" },
                   listItem4 = new ListItem { Text = "4", Value = "4" },
                   listItem5 = new ListItem { Text = "5", Value = "5" },
                   listItem6 = new ListItem { Text = "6", Value = "6" },
                   listItem7 = new ListItem { Text = "7", Value = "7" },
                   listItem8 = new ListItem { Text = "8", Value = "8" },
                   listItem9 = new ListItem { Text = "9", Value = "9" },
                   listItem10 = new ListItem { Text = "10", Value = "10" }
                   ;
            List listl1Q = new List
            {
                Font = "helvetica",
                FontSize = "size11",
                AnchorString = "/l1q/",
                AnchorYOffset = "-10",
                AnchorUnits = "pixels",
                AnchorXOffset = "0",
                ListItems = new List<ListItem>
                {
                    listItem0, listItem1, listItem2,
                    listItem3, listItem4, listItem5, listItem6,
                    listItem7, listItem8, listItem9, listItem10,
                },
                Required = "true",
                TabLabel = "l1q",
            },
            listl2Q = new List
            {
                Font = "helvetica",
                FontSize = "size11",
                AnchorString = "/l2q/",
                AnchorYOffset = "-10",
                AnchorUnits = "pixels",
                AnchorXOffset = "0",
                ListItems = new List<ListItem>
                {
                    listItem0,
                    listItem1,
                    listItem2,
                    listItem3,
                    listItem4,
                    listItem5,
                    listItem6,
                    listItem7,
                    listItem8,
                    listItem9,
                    listItem10,
                },
                Required = "true",
                TabLabel = "l2q",
            };

            // create two formula tabs for the extended price on the line items
            FormulaTab formulal1E = new FormulaTab
            {
                Font = "helvetica",
                FontSize = "size11",
                AnchorString = "/l1e/",
                AnchorYOffset = "-8",
                AnchorUnits = "pixels",
                AnchorXOffset = "105",
                TabLabel = "l1e",
                Formula = $"[l1q] * {l1Price}",
                RoundDecimalPlaces = "0",
                Required = "true",
                Locked = "true",
                DisableAutoSize = "false",
            },
            formulal2E = new FormulaTab
            {
                Font = "helvetica",
                FontSize = "size11",
                AnchorString = "/l2e/",
                AnchorYOffset = "-8",
                AnchorUnits = "pixels",
                AnchorXOffset = "105",
                TabLabel = "l2e",
                Formula = $"[l2q] * {l2Price}",
                RoundDecimalPlaces = "0",
                Required = "true",
                Locked = "true",
                DisableAutoSize = "false",
            },

            // Formula for the total
            formulal3T = new FormulaTab
            {
                Font = "helvetica",
                Bold = "true",
                FontSize = "size12",
                AnchorString = "/l3t/",
                AnchorYOffset = "-8",
                AnchorUnits = "pixels",
                AnchorXOffset = "50",
                TabLabel = "l3t",
                Formula = $"[l1e] + [l2e]",
                RoundDecimalPlaces = "0",
                Required = "true",
                Locked = "true",
                DisableAutoSize = "false",
            };

            // Hidden formula for the payment itself
            FormulaTab formulaPayment = new FormulaTab
            {
                TabLabel = "payment",
                Formula = $"([l1e] + [l2e]) * {currencyMultiplier}",
                RoundDecimalPlaces = "0",
                PaymentDetails = paymentDetails,
                Hidden = "true",
                Required = "true",
                Locked = "true",
                DocumentId = "1",
                PageNumber = "1",
                XPosition = "0",
                YPosition = "0",
            };

            // Tabs are set per recipient / signer
            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere1 },
                ListTabs = new List<List> { listl1Q, listl2Q },
                FormulaTabs = new List<FormulaTab> { formulal1E, formulal2E, formulal3T, formulaPayment },
            };
            signer1.Tabs = signer1Tabs;

            // Add the recipients to the envelope object
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer1 },
                CarbonCopies = new List<CarbonCopy> { cc1 },
            };
            env.Recipients = recipients;

            // Request that the envelope be sent by setting |status| to "sent".
            // To request that the envelope be created as a draft, set to "created"
            env.Status = envStatus;

            return env;
        }

        //ds-snippet-end:eSign14Step3
    }
}
