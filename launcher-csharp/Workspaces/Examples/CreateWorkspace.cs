// <copyright file="CreateWorkspace.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System.Threading.Tasks;
    using Docusign.IAM.SDK;
    using Docusign.IAM.SDK.Models.Components;

    public static class CreateWorkspace
    {
        public static async Task<CreateWorkspaceResponse> CreateWorkspaceAsync(
            string accessToken,
            string accountId,
            string workspaceName)
        {
            //ds-snippet-start:Workspaces1Step2
            var client = CreateAuthenticatedClient(accessToken);
            //ds-snippet-end:Workspaces1Step2

            //ds-snippet-start:Workspaces1Step3
            var createWorkspaceBody = new CreateWorkspaceBody
            {
                Name = workspaceName,
            };
            //ds-snippet-end:Workspaces1Step3
            //ds-snippet-start:Workspaces1Step4
            return await client.Workspaces.Workspaces.CreateWorkspaceAsync(accountId, createWorkspaceBody);
            //ds-snippet-end:Workspaces1Step4
        }

        private static IamClient CreateAuthenticatedClient(string accessToken)
        {
            return IamClient.Builder()
                .WithAccessToken(accessToken)
                .Build();
        }
    }
}
