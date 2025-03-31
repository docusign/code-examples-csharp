// <copyright file="SetConnectedFields.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class SetConnectedFields
    {
        private static readonly HttpClient Client = new HttpClient();

        public static async Task<object> GetConnectedFieldsTabGroupsAsync(string accountId, string accessToken)
        {
            //ds-snippet-start:ConnectedFields1Step3
            var url = $"https://api-d.docusign.com/v1/accounts/{accountId}/connected-fields/tab-groups";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            //ds-snippet-end:ConnectedFields1Step3

            //ds-snippet-start:ConnectedFields1Step2
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            requestMessage.Headers.Add("Accept", "application/json");
            //ds-snippet-end:ConnectedFields1Step2

            try
            {
                //ds-snippet-start:ConnectedFields1Step3
                var response = await Client.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<object>(body);

                return data;
                //ds-snippet-end:ConnectedFields1Step3
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"DocuSign API Request failed: {e.Message}");
            }
        }

        public static JArray FilterData(JArray data)
        {
            //ds-snippet-start:ConnectedFields1Step4
            var filteredData = data.Where(item =>
            {
                var tabs = item["tabs"] as JArray;
                if (tabs == null)
                {
                    return false;
                }

                foreach (var tab in tabs)
                {
                    var extensionData = tab["extensionData"];
                    var tabLabel = tab["tabLabel"]?.ToString();

                    if ((extensionData != null && extensionData["actionContract"]?.ToString().Contains("Verify") == true) ||
                        (tabLabel != null && tabLabel.Contains("connecteddata")))
                    {
                        return true;
                    }
                }

                return false;
            }).ToList();
            //ds-snippet-end:ConnectedFields1Step4

            return new JArray(filteredData);
        }

        public static string SendEnvelopeViaEmail(
            string basePath,
            string accessToken,
            string accountId,
            string signerEmail,
            string signerName,
            string docPdf,
            JObject selectedApp)
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
            JObject selectedApp)
        {
            var appId = selectedApp["appId"]?.ToString() ?? string.Empty;
            JArray tabLabels = (JArray)selectedApp["tabs"];

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

            foreach (var tab in tabLabels)
            {
                var connectionKey = tab["extensionData"]["connectionInstances"] != null ?
                    tab["extensionData"]["connectionInstances"][0]?["connectionKey"]?.ToString() : string.Empty;
                var connectionValue = tab["extensionData"]["connectionInstances"] != null ?
                    tab["extensionData"]["connectionInstances"][0]?["connectionValue"]?.ToString() : string.Empty;
                var extensionGroupId = tab["extensionData"]["extensionGroupId"]?.ToString() ?? string.Empty;
                var publisherName = tab["extensionData"]["publisherName"]?.ToString() ?? string.Empty;
                var applicationName = tab["extensionData"]["applicationName"]?.ToString() ?? string.Empty;
                var actionName = tab["extensionData"]["actionName"]?.ToString() ?? string.Empty;
                var actionInputKey = tab["extensionData"]["actionInputKey"]?.ToString() ?? string.Empty;
                var actionContract = tab["extensionData"]["actionContract"]?.ToString() ?? string.Empty;
                var extensionName = tab["extensionData"]["extensionName"]?.ToString() ?? string.Empty;
                var extensionContract = tab["extensionData"]["extensionContract"]?.ToString() ?? string.Empty;
                var requiredForExtension = tab["extensionData"]["requiredForExtension"]?.ToString() ?? string.Empty;

                var text = new Text
                {
                    RequireInitialOnSharedChange = "false",
                    RequireAll = "false",
                    Name = applicationName,
                    Required = "false",
                    Locked = "false",
                    DisableAutoSize = "false",
                    MaxLength = "4000",
                    TabLabel = tab["tabLabel"].ToString(),
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
                        ExtensionGroupId = extensionGroupId,
                        PublisherName = publisherName,
                        ApplicationId = appId,
                        ApplicationName = applicationName,
                        ActionName = actionName,
                        ActionContract = actionContract,
                        ExtensionName = extensionName,
                        ExtensionContract = extensionContract,
                        RequiredForExtension = requiredForExtension,
                        ActionInputKey = actionInputKey,
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
    }
}
