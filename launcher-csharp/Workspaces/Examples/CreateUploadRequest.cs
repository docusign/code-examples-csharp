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
            //ds-snippet-start:Workspaces5Step2
            var client = CreateAuthenticatedClient(accessToken);
            var dueDate = DateTime.Now.AddDays(7);
            //ds-snippet-end:Workspaces5Step2

            //ds-snippet-start:Workspaces5Step3
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
            //ds-snippet-end:Workspaces5Step3

            //ds-snippet-start:Workspaces5Step4
            return await client.Workspaces.WorkspaceUploadRequest.CreateWorkspaceUploadRequestAsync(
                accountId,
                workspaceId,
                createUploadRequest);
        }

        //ds-snippet-end:Workspaces5Step4

        private static IamClient CreateAuthenticatedClient(string accessToken)
        {
            return IamClient.Builder()
                .WithAccessToken(accessToken)
                .Build();
        }
    }
}
