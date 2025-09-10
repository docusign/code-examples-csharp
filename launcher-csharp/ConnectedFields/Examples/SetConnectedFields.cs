// <copyright file="SetConnectedFields.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using Docusign.IAM.SDK;
    using TabInfo = Docusign.IAM.SDK.Models.Components.TabInfo;

    public static class SetConnectedFields
    {
        public static async Task<List<TabInfo>> GetConnectedFieldsTabGroupsAsync(string basePath, string accountId, string accessToken)
        {
            //ds-snippet-start:ConnectedFields1Step3
            var client = CreateAuthenticatedClient(basePath, accessToken);
            return await client.ConnectedFields.TabInfo.GetConnectedFieldsTabGroupsAsync(accountId);
            //ds-snippet-end:ConnectedFields1Step3
        }

        //ds-snippet-start:ConnectedFields1Step4
        public static List<TabInfo> FilterData(List<TabInfo> connectedFields)
        {
            return connectedFields
                .Where(group => group.Tabs.Any(tab =>
                    tab.ExtensionData?.ActionContract?.Contains("Verify") == true ||
                    tab.TabLabel.Contains("connecteddata")))
                .ToList();
        }

        //ds-snippet-end:ConnectedFields1Step4

        public static string SendEnvelopeViaEmail(
            string basePath,
            string accessToken,
            string accountId,
            string signerEmail,
            string signerName,
            string docPdf,
            TabInfo selectedApp)
        {
            //ds-snippet-start:ConnectedFields1Step6
            EnvelopeDefinition envelopeDefinition = MakeEnvelope(signerEmail, signerName, docPdf, selectedApp);
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, envelopeDefinition);
            //ds-snippet-end:ConnectedFields1Step6
            return results.EnvelopeId;
        }

        //ds-snippet-start:ConnectedFields1Step5
        public static EnvelopeDefinition MakeEnvelope(
            string signerEmail,
            string signerName,
            string docPdf,
            TabInfo selectedApp)
        {
            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition();
            envelopeDefinition.EmailSubject = "Please sign this document set";
            envelopeDefinition.Status = "sent";

            string docPdfBytes = Convert.ToBase64String(System.IO.File.ReadAllBytes(docPdf));
            Document document = new Document
            {
                DocumentBase64 = docPdfBytes,
                Name = "Lorem Ipsum",
                FileExtension = "pdf",
                DocumentId = "1",
            };

            envelopeDefinition.Documents = new List<Document> { document };

            Signer signer = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                RecipientId = "1",
                RoutingOrder = "1",
                Tabs = new Tabs
                {
                    SignHereTabs = new List<SignHere>
                   {
                        new SignHere
                        {
                            AnchorString = "/sn1/",
                            AnchorUnits = "pixels",
                            AnchorYOffset = "10",
                            AnchorXOffset = "20",
                        },
                   },
                    TextTabs = new List<Text>(),
                },
            };

            foreach (var tab in selectedApp.Tabs)
            {
                var extensionData = tab.ExtensionData;
                var connectionKey = extensionData.ConnectionInstances != null ?
                    extensionData.ConnectionInstances[0]?.ConnectionKey : string.Empty;
                var connectionValue = extensionData.ConnectionInstances != null ?
                    extensionData.ConnectionInstances[0]?.ConnectionValue : string.Empty;

                var text = new Text
                {
                    RequireInitialOnSharedChange = "false",
                    RequireAll = "false",
                    Name = extensionData.ApplicationName,
                    Required = "false",
                    Locked = "false",
                    DisableAutoSize = "false",
                    MaxLength = "4000",
                    TabLabel = tab.TabLabel,
                    Font = "lucidaconsole",
                    FontColor = "black",
                    FontSize = "size9",
                    DocumentId = "1",
                    RecipientId = "1",
                    PageNumber = "1",
                    XPosition = "273",
                    YPosition = "191",
                    Width = "84",
                    Height = "22",
                    TemplateRequired = "false",
                    TabType = "text",
                    ExtensionData = new ExtensionData
                    {
                        ExtensionGroupId = extensionData.ExtensionGroupId,
                        PublisherName = extensionData.PublisherName,
                        ApplicationId = selectedApp.AppId,
                        ApplicationName = extensionData.ApplicationName,
                        ActionName = extensionData.ActionName,
                        ActionContract = extensionData.ActionContract,
                        ExtensionName = extensionData.ExtensionName,
                        ExtensionContract = extensionData.ExtensionContract,
                        RequiredForExtension = extensionData.RequiredForExtension.ToString(),
                        ActionInputKey = extensionData.ActionInputKey,
                        ExtensionPolicy = "MustVerifyToSign",
                        ConnectionInstances = new List<ConnectionInstance>
                        {
                            new ConnectionInstance
                            {
                                ConnectionKey = connectionKey,
                                ConnectionValue = connectionValue,
                            },
                        },
                    },
                };
                signer.Tabs.TextTabs.Add(text);
            }

            envelopeDefinition.Recipients = new Recipients
            {
                Signers = new List<Signer>
                {
                    signer,
                },
            };
            return envelopeDefinition;
        }

        //ds-snippet-end:ConnectedFields1Step5

        //ds-snippet-start:ConnectedFields1Step2
        private static IamClient CreateAuthenticatedClient(string basePath, string accessToken) =>
            IamClient.Builder().WithServerUrl(basePath).WithAccessToken(accessToken).Build();
        //ds-snippet-end:ConnectedFields1Step2
    }
}
