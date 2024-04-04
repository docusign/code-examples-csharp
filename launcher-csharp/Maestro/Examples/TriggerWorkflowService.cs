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
    using Newtonsoft.Json.Linq;

    public static class TriggerWorkflowService
    {
        public static WorkflowDefinitionWithId GetWorkFlowDefinition(DocuSignClient docuSignClient, string accountId, string workflowId)
        {
            var maestroApi = new WorkflowManagementApi(docuSignClient);
            return maestroApi.GetWorkflowDefinition(accountId, workflowId);
        }

        public static WorkflowDefinitionList GetWorkFlowDefinitions(DocuSignClient docuSignClient, string accountId)
        {
            //ds-snippet-start:Maestro1Step3
            var maestroApi = new WorkflowManagementApi(docuSignClient);
            var options = new WorkflowManagementApi.GetWorkflowDefinitionsOptions { status = "active" };
            return maestroApi.GetWorkflowDefinitions(accountId, options);
            //ds-snippet-end:Maestro1Step3
        }

        public static string PublishWorkFlow(DocuSignClient docuSignClient, string accountId, string workflowId)
        {
            try
            {
                var maestroApi = new WorkflowManagementApi(docuSignClient);
                maestroApi.PublishOrUnPublishWorkflowDefinition(accountId, workflowId, new DeployRequest());
                return string.Empty;
            }
            catch (ApiException exception)
            {
                if (exception.ErrorContent.Contains("Consent required"))
                {
                    return (string)JObject.Parse(exception.ErrorContent)["consentUrl"];
                }
                else
                {
                    throw;
                }
            }
        }

        public static TriggerWorkflowViaPostResponse TriggerWorkflow(DocuSignClient docuSignClient, string accountId, Uri triggerUrl, WorkflowTriggerModel model)
        {
            //ds-snippet-start:Maestro1Step4
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
            //ds-snippet-end:Maestro1Step4
            //ds-snippet-start:Maestro1Step5
            return maestroApi.TriggerWorkflow(accountId, payload, options);
            //ds-snippet-end:Maestro1Step5
        }
    }
}
