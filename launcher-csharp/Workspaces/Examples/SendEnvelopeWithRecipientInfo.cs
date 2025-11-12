// <copyright file="SendEnvelopeWithRecipientInfo.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Model;
    using Docusign.IAM.SDK;
    using Docusign.IAM.SDK.Models.Components;

    public static class SendEnvelopeWithRecipientInfo
    {
        public static async Task<CreateWorkspaceEnvelopeResponse> CreateEnvelopeAsync(
            string accessToken,
            string accountId,
            string documentId,
            string workspaceId)
        {
            //ds-snippet-start:Workspaces3Step2
            var client = CreateAuthenticatedClient(accessToken);
            //ds-snippet-end:Workspaces3Step2

            //ds-snippet-start:Workspaces3Step3
            var workspaceEnvelopeForCreate = new WorkspaceEnvelopeForCreate
            {
                EnvelopeName = "Example Workspace Envelope",
                DocumentIds = new List<string> { documentId },
            };
            //ds-snippet-end:Workspaces3Step3

            //ds-snippet-start:Workspaces3Step4
            return await client.Workspaces.Workspaces.CreateWorkspaceEnvelopeAsync(
                accountId,
                workspaceId,
                workspaceEnvelopeForCreate);
            //ds-snippet-end:Workspaces3Step4
        }

        //ds-snippet-start:Workspaces3Step6
        public static async Task<EnvelopeUpdateSummary> SendEnvelopeAsync(
            string basePath,
            string accessToken,
            string accountId,
            string envelopeId,
            string signerEmail,
            string signerName)
        {
            var apiClient = new eSign.Client.DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var envelopesApi = new EnvelopesApi(apiClient);
            var envelope = MakeEnvelope(signerEmail, signerName);

            return await envelopesApi.UpdateAsync(
                accountId,
                envelopeId,
                new Envelope
                {
                    EnvelopeId = envelopeId,
                    Status = envelope.Status,
                    Recipients = envelope.Recipients,
                });
        }

        //ds-snippet-end:Workspaces3Step6

        //ds-snippet-start:Workspaces3Step5
        private static EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName)
        {
            var signHere = new SignHere
            {
                AnchorString = "/sn1/",
                AnchorUnits = "pixels",
                AnchorXOffset = "20",
                AnchorYOffset = "10",
            };

            var tabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere },
            };

            var signer = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                RecipientId = "1",
                RoutingOrder = "1",
                Tabs = tabs,
            };

            var recipients = new Recipients
            {
                Signers = new List<Signer> { signer },
            };

            var envelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = "Please sign this document",
                Recipients = recipients,
                Status = "sent",
            };

            return envelopeDefinition;
        }

        //ds-snippet-end:Workspaces3Step5
        private static IamClient CreateAuthenticatedClient(string accessToken)
        {
            return IamClient.Builder()
                .WithAccessToken(accessToken)
                .Build();
        }
    }
}
