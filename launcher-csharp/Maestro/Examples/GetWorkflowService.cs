// <copyright file="GetWorkflowService.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.WebForms.Examples
{
    using DocuSign.Maestro.Api;
    using DocuSign.Maestro.Client;
    using DocuSign.Maestro.Model;

    public static class GetWorkflowService
    {
        public static WorkflowInstance GetWorkFlowInstance(DocuSignClient docuSignClient, string accountId, string workflowId, string instanceId)
        {
            var maestroApi = new WorkflowInstanceManagementApi(docuSignClient);
            return maestroApi.GetWorkflowInstance(accountId, workflowId, instanceId);
        }
    }
}
