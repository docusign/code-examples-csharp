// <copyright file="CreateUploadRequest.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Docusign.IAM.SDK;
    using Docusign.IAM.SDK.Models.Components;

    public static class CreateUploadRequest
    {
        public static async Task<GetWorkspaceUploadRequestResponse> CreateUploadRequestWithDueDate(
            string accessToken,
            string accountId,
            string workspaceId,
            string creatorId,
            string assigneeEmail)
        {
            var client = CreateAuthenticatedClient(accessToken);
            var dueDate = DateTime.Now.AddDays(7);

            var createUploadRequest = new CreateWorkspaceUploadRequestBody()
            {
                Name = $"Upload Request example {dueDate}",
                Description = "This is an example upload request created via the workspaces API",
                DueDate = dueDate,
                Assignments = new List<CreateWorkspaceUploadRequestAssignment>
                {
                    new CreateWorkspaceUploadRequestAssignment
                    {
                        Email = assigneeEmail,
                        UploadRequestResponsibilityTypeId = WorkspaceUploadRequestResponsibilityType.Assignee,
                        FirstName = "Test",
                        LastName = "User",
                    },
                    new CreateWorkspaceUploadRequestAssignment
                    {
                        AssigneeUserId = creatorId,
                        UploadRequestResponsibilityTypeId = WorkspaceUploadRequestResponsibilityType.Watcher,
                    },
                },
                Status = WorkspaceUploadRequestStatus.Draft,
            };

            return await client.Workspaces.WorkspaceUploadRequest.CreateWorkspaceUploadRequestAsync(
                accountId,
                workspaceId,
                createUploadRequest);
        }

        private static IamClient CreateAuthenticatedClient(string accessToken)
        {
            return IamClient.Builder()
                .WithAccessToken(accessToken)
                .Build();
        }
    }
}
