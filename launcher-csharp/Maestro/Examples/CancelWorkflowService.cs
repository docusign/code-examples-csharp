// <copyright file="CancelWorkflowService.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.WebForms.Examples
{
    using DocuSign.Maestro.Api;
    using DocuSign.Maestro.Client;
    using DocuSign.Maestro.Model;

    public static class CancelWorkflowService
    {
        public static CancelResponse CancelWorkflow(DocuSignClient docuSignClient, string accountId, string instanceId)
        {
            var maestroApi = new WorkflowInstanceManagementApi(docuSignClient);
            return maestroApi.CancelWorkflowInstance(accountId, instanceId);
        }
    }
}
