// <copyright file="ResponsiveSigning.cs" company="DocuSign">
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

    public static class ResponsiveSigning
    {
        public static string CreateEnvelopeFromHtml(
            string signerEmail,
            string signerName,
            string ccEmail,
            string ccName,
            string signerClientId,
            string accessToken,
            string basePath,
            string accountId,
            string returnUrl,
            string pingUrl = null)
        {
            //ds-snippet-start:eSign38Step3
            EnvelopeDefinition envelope = MakeEnvelope(signerEmail, signerName, ccEmail, ccName, signerClientId);

            DocuSignClient docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envelope);
            //ds-snippet-end:eSign38Step3
            string envelopeId = envelopeSummary.EnvelopeId;

            RecipientViewRequest viewRequest = MakeRecipientViewRequest(signerEmail, signerName, returnUrl, signerClientId, pingUrl);

            ViewUrl viewUrl = envelopesApi.CreateRecipientView(accountId, envelopeId, viewRequest);

            return viewUrl.Url;
        }

        private static RecipientViewRequest MakeRecipientViewRequest(
            string signerEmail,
            string signerName,
            string returnUrl,
            string signerClientId,
            string pingUrl = null)
        {
            RecipientViewRequest viewRequest = new RecipientViewRequest();
            viewRequest.ReturnUrl = returnUrl + "?state=123";
            viewRequest.AuthenticationMethod = "none";

            viewRequest.Email = signerEmail;
            viewRequest.UserName = signerName;
            viewRequest.ClientUserId = signerClientId;

            if (pingUrl != null)
            {
                viewRequest.PingFrequency = "600";
                viewRequest.PingUrl = pingUrl;
            }

            return viewRequest;
        }

        //ds-snippet-start:eSign38Step2
        private static EnvelopeDefinition MakeEnvelope(
            string signerEmail,
            string signerName,
            string ccEmail,
            string ccName,
            string signerClientId)
        {
            int price1 = 5;
            int price2 = 150;

            FormulaTab formulaTab1 = new FormulaTab
            {
                Font = "helvetica",
                FontSize = "size11",
                FontColor = "black",
                AnchorString = "/l1e/",
                AnchorYOffset = "-8",
                AnchorUnits = "pixels",
                AnchorXOffset = "105",
                TabLabel = "l1e",
                Formula = $"[l1q] * {price1}",
                RoundDecimalPlaces = "0",
                Required = "true",
                Locked = "true",
                DisableAutoSize = "false",
            };

            FormulaTab formulaTab2 = new FormulaTab
            {
                Font = "helvetica",
                FontSize = "size11",
                FontColor = "black",
                AnchorString = "/l2e/",
                AnchorYOffset = "-8",
                AnchorUnits = "pixels",
                AnchorXOffset = "105",
                TabLabel = "l2e",
                Formula = $"[l2q] * {price2}",
                RoundDecimalPlaces = "0",
                Required = "true",
                Locked = "true",
                DisableAutoSize = "false",
            };

            FormulaTab formulaTab3 = new FormulaTab
            {
                Font = "helvetica",
                FontSize = "size11",
                FontColor = "black",
                AnchorString = "/l3t/",
                AnchorYOffset = "-8",
                AnchorUnits = "pixels",
                AnchorXOffset = "105",
                TabLabel = "l3t",
                Formula = "[l1e] + [l2e]",
                RoundDecimalPlaces = "0",
                Required = "true",
                Locked = "true",
                DisableAutoSize = "false",
                Bold = "true",
            };

            Tabs tabs = new Tabs
            {
                FormulaTabs = new List<FormulaTab> { formulaTab1, formulaTab2, formulaTab3 },
            };

            Signer signer = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                ClientUserId = signerClientId,
                RecipientId = "1",
                RoutingOrder = "1",
                RoleName = "Signer",
                Tabs = tabs,
            };

            CarbonCopy cc = new CarbonCopy
            {
                Email = ccEmail,
                Name = ccName,
                RecipientId = "2",
                RoutingOrder = "1",
            };

            string htmlMarkup = System.IO.File.ReadAllText("order_form.html");

            string htmlWithData = htmlMarkup.Replace("{signerName}", signerName)
                    .Replace("{signerEmail}", signerEmail)
                    .Replace("{ccName}", ccName)
                    .Replace("{ccEmail}", ccEmail)
                    .Replace("/sn1/", "<ds-signature data-ds-role=\"Signer\"/>")
                    .Replace("/l1q/", " <input data-ds-type=\"number\" name=\"l1q\"/>")
                    .Replace("/l2q/", " <input data-ds-type=\"number\" name=\"l2q\"/>");

            return new EnvelopeDefinition()
            {
                EmailSubject = "Example Signing Document",
                Documents = new List<Document>
                {
                    new Document
                    {
                        Name = "Lorem Ipsum",
                        DocumentId = "1",
                        HtmlDefinition = new DocumentHtmlDefinition
                        {
                            Source = htmlWithData,
                        },
                    },
                },
                Recipients = new Recipients
                {
                    Signers = new List<Signer> { signer },
                    CarbonCopies = new List<CarbonCopy> { cc },
                },
                Status = "sent",
            };
        }

        //ds-snippet-end:eSign38Step2
    }
}
