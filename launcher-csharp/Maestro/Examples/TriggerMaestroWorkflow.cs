// <copyright file="TriggerMaestroWorkflow.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Docusign.IAM.SDK;
    using Docusign.IAM.SDK.Models.Components;

    public static class TriggerMaestroWorkflow
    {
        /// <summary>
        /// Returns a list of Maestro workflows for the account.
        /// </summary>
        /// <returns>WorkflowsListSuccess</returns>
        public static async Task<WorkflowsListSuccess> GetMaestroWorkflow(
            string basePath,
            string accessToken,
            string accountId)
        {
            var client = CreateAuthenticatedClient(basePath, accessToken);
            return await client.Maestro.Workflows.GetWorkflowsListAsync(accountId);
        }

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

            var triggerInputs = new Dictionary<string, TriggerInputs>
            {
                { "signerName", CreateTriggerInput(signerName) },
                { "signerEmail", CreateTriggerInput(signerEmail) },
                { "ccName", CreateTriggerInput(ccName) },
                { "ccEmail", CreateTriggerInput(ccEmail) },
            };

            var triggerWorkflow = new TriggerWorkflow
            {
                InstanceName = instanceName,
                TriggerInputs = triggerInputs,
            };

            return await client.Maestro.Workflows.TriggerWorkflowAsync(
                accountId,
                workflowId,
                triggerWorkflow);
        }

        /// <summary>
        /// Creates an authenticated IAM client.
        /// </summary>
        private static IamClient CreateAuthenticatedClient(string basePath, string accessToken)
        {
            return IamClient.Builder()
                .WithServerUrl(basePath)
                .WithAccessToken(accessToken)
                .Build();
        }

        /// <summary>
        /// Creates a TriggerInputs object for string inputs.
        /// </summary>
        private static TriggerInputs CreateTriggerInput(string value)
        {
            return new TriggerInputs(TriggerInputsType.Str) { Str = value };
        }
    }
}
