using System;
using System.Collections.Generic;
using DocuSign.Rooms.Model;

namespace DocuSign.CodeExamples.Rooms.Models
{
    public class FormFormGroupModel
    {
        public string FormGroupId { get; set; }

        public List<FormGroupSummary> FormGroups { get; set; }

        public Guid FormId { get; set; }

        public List<FormSummary> Forms { get; set; }
    }
}
