// <copyright file="SetConnectedFields.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using DocuSign.CodeExamples.ConnectedFields.Models;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using static System.Net.Mime.MediaTypeNames;

    public static class SetConnectedFields
    {
        private static readonly HttpClient Client = new HttpClient();

        public static async Task<ExtensionApps> GetConnectedFieldsAsync(string accessToken, string accountId)
        {
            string baseUrl = "https://api-d.docusign.com/v1";
            string requestUrl = $"{baseUrl}/accounts/{accountId}/connected-fields/tab-groups";

            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("Content-Type", "application/json");

                HttpResponseMessage response = await Client.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ExtensionApps>(responseBody);
            }
        }

        public static string SendEnvelopeViaEmail(string signerEmail, string signerName, ExtensionApp extension, string basePath, string accessToken, string accountId, string docPdf)
        {
            EnvelopeDefinition env = MakeEnvelope(signerEmail, signerName, extension,  docPdf);
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, env);
            return results.EnvelopeId;
        }

        public static EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName, ExtensionApp extension, string docPdf)
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
                Tabs = new eSign.Model.Tabs
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
                   TextTabs = new List<eSign.Model.Text>
                   {
                        new eSign.Model.Text
                        {
                            RequireInitialOnSharedChange = "false",
                            RequireAll = "false",
                            Name = extension.Tabs.First().ExtensionData.ApplicationName,
                            Required = "false",
                            Locked = "false",
                            DisableAutoSize = "false",
                            MaxLength = "4000",
                            TabLabel = extension.Tabs.First().TabLabel,
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
                            //add extension code here
                        },
                   },
                },
            };

            return envelopeDefinition;
        }
    }
}
