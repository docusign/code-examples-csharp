// <copyright file="IRequestItemsService.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples
{
    using System;
    using System.Collections.Generic;
    using DocuSign.CodeExamples.Models;
    using Docusign.IAM.SDK.Models.Components;
    using Microsoft.Extensions.Configuration;

    public interface IRequestItemsService
    {
        public string EgName { get; set; }

        public Session Session { get; set; }

        public User User { get; set; }

        public IConfiguration Configuration { get; set; }

        public Guid? OrganizationId { get; set; }

        public string AuthenticatedUserEmail { get; set; }

        string EnvelopeId { get; set; }

        public List<TabInfo> ExtensionApps { get; set; }

        public string DocumentId { get; set; }

        public string ClickwrapId { get; set; }

        public string ClickwrapName { get; set; }

        public EnvelopeDocuments EnvelopeDocuments { get; set; }

        public string TemplateId { get; set; }

        public string WebFormsTemplateId { get; set; }

        public string WorkflowId { get; set; }

        public string WorkspaceId { get; set; }

        public string CreatorId { get; set; }

        public string WorkspaceDocumentId { get; set; }

        public bool IsWorkflowPublished { get; set; }

        public string InstanceId { get; set; }

        public string PausedEnvelopeId { get; set; }

        public string Status { get; set; }

        public string EmailAddress { get; set; }

        public void UpdateUserFromJwt();

        public string IdentifyApiOfCodeExample(string eg);

        public void Logout();

        public bool CheckToken(int bufferMin);
    }
}