// <copyright file="ResumeWorkflow.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System.Threading.Tasks;
    using Docusign.IAM.SDK;
    using Docusign.IAM.SDK.Models.Components;

    public static class ResumeWorkflow
    {
        public static async Task<ResumeNewWorkflowInstancesSuccess> ResumeMaestroWorkflow(
            string basePath,
            string accessToken,
            string accountId,
            string workflowId)
        {
            var client = CreateAuthenticatedClient(basePath, accessToken);
            return await client.Maestro.Workflows.ResumePausedWorkflowAsync(accountId, workflowId);
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
    }
}
