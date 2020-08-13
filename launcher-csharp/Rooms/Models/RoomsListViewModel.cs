using System.Collections.Generic;
using DocuSign.Rooms.Model;

namespace eg_03_csharp_auth_code_grant_core.Rooms.Models
{
    public class RoomsListViewModel
    {
        public int RoomId { get; set; }

        public List<RoomSummary> Rooms { get; set; }
    }
}