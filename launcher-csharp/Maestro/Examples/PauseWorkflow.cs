// <copyright file="PauseWorkflow.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System.Threading.Tasks;
    using Docusign.IAM.SDK;
    using Docusign.IAM.SDK.Models.Components;

    public static class PauseWorkflow
    {
        //ds-snippet-start:Maestro2Step3
        public static async Task<PauseNewWorkflowInstancesSuccess> PauseMaestroWorkflow(
            string basePath,
            string accessToken,
            string accountId,
            string workflowId)
        {
            var client = CreateAuthenticatedClient(basePath, accessToken);
            return await client.Maestro.Workflows.PauseNewWorkflowInstancesAsync(accountId, workflowId);
        }

        //ds-snippet-end:Maestro2Step3

        /// <summary>
        /// Creates an authenticated IAM client.
        /// </summary>
        //ds-snippet-start:Maestro2Step2
        private static IamClient CreateAuthenticatedClient(string basePath, string accessToken)
        {
            return IamClient.Builder()
                .WithServerUrl(basePath)
                .WithAccessToken(accessToken)
                .Build();
        }

        //ds-snippet-end:Maestro2Step2
    }
}
