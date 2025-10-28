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
            var client = CreateAuthenticatedClient(accessToken);

            var createWorkspaceBody = new CreateWorkspaceBody
            {
                Name = workspaceName,
            };
            return await client.Workspaces.Workspaces.CreateWorkspaceAsync(accountId, createWorkspaceBody);
        }

        private static IamClient CreateAuthenticatedClient(string accessToken)
        {
            return IamClient.Builder()
                .WithAccessToken(accessToken)
                .Build();
        }
    }
}
