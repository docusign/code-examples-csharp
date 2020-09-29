using System;
using System.Collections.Generic;
using DocuSign.Rooms.Model;

namespace DocuSign.CodeExamples.Rooms.Models
{
    public class RoomFormModel
    {
        public int RoomId { get; set; }

        public List<RoomSummary> Rooms { get; set; }

        public Guid FormId { get; set; }

        public List<FormSummary> Forms { get; set; }
    }
}