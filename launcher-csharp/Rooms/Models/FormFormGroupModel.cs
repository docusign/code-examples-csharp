// <copyright file="FormFormGroupModel.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Models
{
    using System;
    using System.Collections.Generic;
    using DocuSign.Rooms.Model;

    public class FormFormGroupModel
    {
        public string FormGroupId { get; set; }

        public List<FormGroupSummary> FormGroups { get; set; }

        public Guid FormId { get; set; }

        public List<FormSummary> Forms { get; set; }
    }
}
