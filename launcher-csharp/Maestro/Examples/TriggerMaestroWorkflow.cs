// <copyright file="TriggerMaestroWorkflow.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Docusign.IAM.SDK;
    using Docusign.IAM.SDK.Models.Components;
    using Docusign.IAM.SDK.Models.Requests;
    using Newtonsoft.Json.Linq;

    public static class TriggerMaestroWorkflow
    {
        private const string MaestroBasePath = "https://demo.services.docusign.net/aow-manage/v1.0";

        /// <summary>
        /// Returns a list of Maestro workflows for the account.
        /// </summary>
        /// <returns>WorkflowsListSuccess</returns>
        public static async Task<WorkflowsListSuccess> GetMaestroWorkflow(
            string basePath,
            string accessToken,
            string accountId)
        //ds-snippet-start:Maestro1Step3
        {
            var client = CreateAuthenticatedClient(basePath, accessToken);
            return await client.Maestro.Workflows.GetWorkflowsListAsync(accountId, Status.Active);
        }

        //ds-snippet-start:Maestro1Step3

        /// <summary>
        /// Triggers a specific Maestro workflow instance.
        /// </summary>
        /// <returns>Instance URL.</returns>
        public static async Task<TriggerWorkflowSuccess> TriggerWorkflowInstance(
            string basePath,
            string accessToken,
            string accountId,
            string workflowId,
            string signerEmail,
            string signerName,
            string ccEmail,
            string ccName,
            string instanceName)
        {
            var client = CreateAuthenticatedClient(basePath, accessToken);

            //ds-snippet-start:Maestro1Step4
            var triggerInputs = new Dictionary<string, TriggerInputs>
            {
                { "signerName", CreateTriggerInput(signerName) },
                { "signerEmail", CreateTriggerInput(signerEmail) },
                { "ccName", CreateTriggerInput(ccName) },
                { "ccEmail", CreateTriggerInput(ccEmail) },
            };
            //ds-snippet-end:Maestro1Step4

            //ds-snippet-start:Maestro1Step5
            var triggerWorkflow = new TriggerWorkflow
            {
                InstanceName = instanceName,
                TriggerInputs = triggerInputs,
            };

            return await client.Maestro.Workflows.TriggerWorkflowAsync(
                accountId,
                workflowId,
                triggerWorkflow);
            //ds-snippet-end:Maestro1Step5
        }

        /// <summary>
        /// Create a Maestro workflow.
        /// </summary>
        /// <returns>ID of the maestro workflow.</returns>
        public static async Task<string> CreateWorkflowAsync(
            string accessToken,
            string accountId,
            string templateId,
            string fileLocation)
        {
            string signerId = Guid.NewGuid().ToString();
            string ccId = Guid.NewGuid().ToString();
            var triggerId = "wfTrigger";

            var requestJson = PrepareWorkflowDefinition(fileLocation, templateId, signerId, ccId, triggerId, accountId);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(requestJson);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var url = $"{MaestroBasePath}/management/accounts/{accountId}/workflowDefinitions";

                var response = await httpClient.PostAsync(url, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Unable to create a new workflow" + responseBody);
                }

                var jsonResponse = JObject.Parse(responseBody);
                return jsonResponse["workflowDefinitionId"]?.ToString();
            }
        }

        /// <summary>
        /// Publish a Maestro workflow.
        /// </summary>
        /// <returns>Consent URL in case the workflow is yet not published.</returns>
        public static async Task<string> PublishWorkflowAsync(
            string accountId,
            string workflowId,
            string accessToken)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var url =
                $"{MaestroBasePath}/management/accounts/{accountId}/workflowDefinitions/{workflowId}/publish?isPreRunCheck=true";

            var response = await httpClient.PostAsync(url, null);
            var responseBody = await response.Content.ReadAsStringAsync();

            if ((int)response.StatusCode > 201)
            {
                var jsonResponse = JObject.Parse(responseBody);
                var message = jsonResponse["message"]?.ToString();
                var consentUrl = jsonResponse["consentUrl"]?.ToString();

                if (message == "Consent required" && !string.IsNullOrEmpty(consentUrl))
                {
                    return consentUrl;
                }

                throw new Exception("Error publishing workflow. " + message);
            }

            await Task.Delay(8000);
            return string.Empty;
        }

        /// <summary>
        /// Creates an authenticated IAM client.
        /// </summary>
        //ds-snippet-start:Maestro1Step2
        private static IamClient CreateAuthenticatedClient(string basePath, string accessToken)
        {
            return IamClient.Builder()
                .WithServerUrl(basePath)
                .WithAccessToken(accessToken)
                .Build();
        }

        //ds-snippet-end:Maestro1Step2

        /// <summary>
        /// Creates a TriggerInputs object for string inputs.
        /// </summary>
        private static TriggerInputs CreateTriggerInput(string value)
        {
            return new TriggerInputs(TriggerInputsType.Str) { Str = value };
        }

        /// <summary>
        /// Prepares a Maestro workflow definition from file.
        /// </summary>
        /// <returns>Workflow in string format</returns>
        private static string PrepareWorkflowDefinition(
            string fileLocation,
            string templateId,
            string signerId,
            string ccId,
            string triggerId,
            string accountId)
        {
            string templateIdTargetString = "TEMPLATE_ID";
            string accountIdTargetString = "ACCOUNT_ID";
            string signerIdTargetString = "SIGNER_ID";
            string ccIdTargetString = "CC_ID";
            string triggerIdTargetString = "TRIGGER_ID";

            try
            {
                string workflowDefinition = File.ReadAllText(fileLocation);
                string workflowDefinitionWithData = workflowDefinition
                    .Replace(templateIdTargetString, templateId)
                    .Replace(accountIdTargetString, accountId)
                    .Replace(signerIdTargetString, signerId)
                    .Replace(ccIdTargetString, ccId)
                    .Replace(triggerIdTargetString, triggerId);

                return workflowDefinitionWithData;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }
    }
}
