using System.Collections.Generic;
using DocuSign.Rooms.Model;

namespace DocuSign.CodeExamples.Rooms.Models
{
    public class OfficeAccessModel
    {
        public List<OfficeSummary> Offices { get; set; }
        public List<FormGroupSummary> FormGroups { get; set; }
        public string FormGroupId { get; set; }
        public int? OfficeId { get; set; }
    }
}