// <copyright file="CancelWorkflowInstance.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System.Threading.Tasks;
    using Docusign.IAM.SDK;
    using Docusign.IAM.SDK.Models.Components;

    public static class CancelWorkflowInstance
    {
        //ds-snippet-start:Maestro4Step3
        public static async Task<CancelWorkflowInstanceResponse> CancelInstanceMaestroWorkflow(
            string basePath,
            string accessToken,
            string accountId,
            string workflowId,
            string workflowInstanceId)
        {
            var client = CreateAuthenticatedClient(basePath, accessToken);
            return await client.Maestro.WorkflowInstanceManagement.CancelWorkflowInstanceAsync(accountId, workflowId, workflowInstanceId);
        }
        //ds-snippet-end:Maestro4Step3

        /// <summary>
        /// Creates an authenticated IAM client.
        /// </summary>
        //ds-snippet-start:Maestro4Step2
        private static IamClient CreateAuthenticatedClient(string basePath, string accessToken)
        {
            return IamClient.Builder()
                .WithServerUrl(basePath)
                .WithAccessToken(accessToken)
                .Build();
        }
        //ds-snippet-end:Maestro4Step2
    }
}
