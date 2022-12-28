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
        public static string CreateEnvelopeFromHTML(
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
            EnvelopeDefinition envelope = MakeEnvelope(signerEmail, signerName, ccEmail, ccName, signerClientId);

            DocuSignClient docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envelope);
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

        private static EnvelopeDefinition MakeEnvelope(
            string signerEmail,
            string signerName,
            string ccEmail,
            string ccName,
            string signerClientId)
        {
            Signer signer = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                ClientUserId = signerClientId,
                RecipientId = "1",
                RoutingOrder = "1",
                RoleName = "Signer",
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
                    .Replace("/l1q/", " <input data-ds-type=\"number\"/>")
                    .Replace("/l2q/", " <input data-ds-type=\"number\"/>");

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
    }
}
