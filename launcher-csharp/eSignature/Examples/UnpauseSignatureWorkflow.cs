// <copyright file="UnpauseSignatureWorkflow.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using System;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using static DocuSign.eSign.Api.EnvelopesApi;

    public class UnpauseSignatureWorkflow
    {
        /// <summary>
        /// Unpauses signature workflow
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="pausedEnvelopeId">The Envelope ID which workflow will be unpaused</param>
        /// <returns>The update summary of the envelopes</returns>
        public static EnvelopeUpdateSummary UnpauseWorkflow(string accessToken, string basePath, string accountId, string pausedEnvelopeId)
        {
            // Construct your API headers
            //ds-snippet-start:eSign33Step2
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            //ds-snippet-end:eSign33Step2

            // Construct request body
            var envelope = MakeEnvelope();

            // Call the eSignature REST API
            EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);

            //ds-snippet-start:eSign33Step4
            var updateOptions = new UpdateOptions() { resendEnvelope = "true" };
            var response = envelopesApi.UpdateWithHttpInfo(accountId, pausedEnvelopeId, envelope, updateOptions);
            response.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining);
            response.Headers.TryGetValue("X-RateLimit-Reset", out string reset);

            DateTime resetDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset)).UtcDateTime;

            Console.WriteLine("API calls remaining: " + remaining);
            Console.WriteLine("Next Reset: " + resetDate);
            return response.Data;
            //ds-snippet-end:eSign33Step4
        }

        //ds-snippet-start:eSign33Step3
        private static Envelope MakeEnvelope()
        {
            return new Envelope
            {
                Workflow = new Workflow()
                {
                    WorkflowStatus = "in_progress",
                },
            };
        }

        //ds-snippet-end:eSign33Step3
    }
}
