using System;
using System.Collections.Generic;
using DocuSign.Rooms.Model;

namespace DocuSign.CodeExamples.Rooms.Models
{
    public class RoomDocumentModel
    {
        public int? RoomId { get; set; }

        public List<RoomSummary> Rooms { get; set; }

        public Guid DocumentId { get; set; }

        public List<RoomDocument> Documents { get; set; }
    }
}