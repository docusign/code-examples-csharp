// <copyright file="AddDocumentToWorkspaces.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System.IO;
    using System.Threading.Tasks;
    using Docusign.IAM.SDK;
    using Docusign.IAM.SDK.Models.Components;

    public static class AddDocumentToWorkspaces
    {
        public static async Task<CreateWorkspaceDocumentResponse> AddDocumentToWorkspaceAsync(
            string accessToken,
            string accountId,
            string workspaceId,
            string documentName,
            string documentPath)
        {
            var client = CreateAuthenticatedClient(accessToken);

            byte[] fileBytes = File.ReadAllBytes(documentPath);

            var addWorkspaceDocumentRequest = new AddWorkspaceDocumentRequest
            {
                File = new AddWorkspaceDocumentRequestFile
                {
                    FileName = documentName,
                    Content = fileBytes,
                },
            };

            return await client.Workspaces.WorkspaceDocuments.AddWorkspaceDocumentAsync(
                accountId,
                workspaceId,
                addWorkspaceDocumentRequest);
        }

        private static IamClient CreateAuthenticatedClient(string accessToken)
        {
            return IamClient.Builder()
                .WithAccessToken(accessToken)
                .Build();
        }
    }
}
