// <copyright file="WorkflowTriggerModel.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Maestro.Models
{
    public class WorkflowTriggerModel
    {
        public string InstanceName { get; set; }

        public string SignerEmail { get; set; }

        public string SignerName { get; set; }

        public string CCEmail { get; set; }

        public string CCName { get; set; }
    }
}