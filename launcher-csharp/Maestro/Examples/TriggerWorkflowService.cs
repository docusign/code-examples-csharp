// <copyright file="TriggerWorkflowService.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.WebForms.Examples
{
    using System;
    using System.Web;
    using DocuSign.CodeExamples.Maestro.Models;
    using DocuSign.Maestro.Api;
    using DocuSign.Maestro.Client;
    using DocuSign.Maestro.Model;

    public static class TriggerWorkflowService
    {
        public static WorkflowDefinitionWithId GetWorkFlowDefinition(DocuSignClient docuSignClient, string accountId, string workflowId)
        {
            var maestroApi = new WorkflowManagementApi(docuSignClient);
            return maestroApi.GetWorkflowDefinition(accountId, workflowId);
        }

        public static TriggerWorkflowViaPostResponse TriggerWorkflow(DocuSignClient docuSignClient, string accountId, Uri triggerUrl, WorkflowTriggerModel model)
        {
            var maestroApi = new WorkflowTriggerApi(docuSignClient);
            var payload = new TriggerPayload
            {
                InstanceName = model.InstanceName,
                Metadata = new object(),
                Participants = new object(),
                Payload = new
                {
                    signerEmail = model.SignerEmail,
                    signerName = model.SignerName,
                    ccEmail = model.CCEmail,
                    ccName = model.CCName,
                },
            };

            var uriParameters = HttpUtility.ParseQueryString(triggerUrl.Query);
            var options = new WorkflowTriggerApi.TriggerWorkflowOptions
            {
                mtid = uriParameters["mtid"],
                mtsec = uriParameters["mtsec"],
            };
            return maestroApi.TriggerWorkflow(accountId, payload, options);
        }
    }
}
