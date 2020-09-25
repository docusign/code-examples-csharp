using System.Collections.Generic;
using DocuSign.Rooms.Model;

namespace DocuSign.CodeExamples.Rooms.Models
{
    public class RoomModel
    {
        public string Name { get; set; }

        public int TemplateId { get; set; }

        public List<RoomTemplate> Templates { get; set; }
    }
}
