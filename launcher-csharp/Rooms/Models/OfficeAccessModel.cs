// <copyright file="OfficeAccessModel.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Models
{
    using System.Collections.Generic;
    using DocuSign.Rooms.Model;

    public class OfficeAccessModel
    {
        public List<OfficeSummary> Offices { get; set; }

        public List<FormGroupSummary> FormGroups { get; set; }

        public string FormGroupId { get; set; }

        public int? OfficeId { get; set; }
    }
}