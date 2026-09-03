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
            //ds-snippet-start:Workspaces2Step2
            var client = CreateAuthenticatedClient(accessToken);
            //ds-snippet-end:Workspaces2Step2

            // Guard against path traversal: only allow reading a file that already exists
            // directly under the application's base directory. The path passed to the file
            // API is taken from the trusted directory listing (not from user input); the
            // user-supplied value is used solely to select a matching file name.
            string requestedName = Path.GetFileName(documentPath);
            string fullPath = null;
            foreach (var candidate in Directory.EnumerateFiles(Directory.GetCurrentDirectory()))
            {
                if (string.Equals(Path.GetFileName(candidate), requestedName, System.StringComparison.Ordinal))
                {
                    fullPath = candidate;
                    break;
                }
            }

            if (fullPath == null)
            {
                throw new System.ArgumentException("Invalid document path.", nameof(documentPath));
            }

            byte[] fileBytes = File.ReadAllBytes(fullPath);

            //ds-snippet-start:Workspaces2Step3
            var addWorkspaceDocumentRequest = new AddWorkspaceDocumentRequest
            {
                File = new AddWorkspaceDocumentRequestFile
                {
                    FileName = documentName,
                    Content = fileBytes,
                },
            };
            //ds-snippet-end:Workspaces2Step3

            //ds-snippet-start:Workspaces2Step4
            return await client.Workspaces.WorkspaceDocuments.AddWorkspaceDocumentAsync(
                accountId,
                workspaceId,
                addWorkspaceDocumentRequest);
            //ds-snippet-end:Workspaces2Step4
        }

        private static IamClient CreateAuthenticatedClient(string accessToken)
        {
            return IamClient.Builder()
                .WithAccessToken(accessToken)
                .Build();
        }
    }
}
