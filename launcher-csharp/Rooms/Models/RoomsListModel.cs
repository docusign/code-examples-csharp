using System.Collections.Generic;
using DocuSign.Rooms.Model;

namespace DocuSign.CodeExamples.Rooms.Models
{
    public class RoomsListModel
    {
        public int RoomId { get; set; }

        public List<RoomSummary> Rooms { get; set; }
    }
}