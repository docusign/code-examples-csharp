using System.Collections.Generic; 
using DocuSign.Rooms.Model;

namespace eg_03_csharp_auth_code_grant_core.Rooms.Models
{
    public class RoomModel
    {
        public string Name { get; set; }

        public int TemplateId { get; set; }

        public List<RoomTemplate> Templates { get; set; }
    }
}
