using System;

namespace eg_03_csharp_auth_code_grant_core.Rooms.Models
{
    public class RoomFilterModel
    {
        public RoomFilterModel()
        {
            FieldDataChangedStartDate = DateTime.Now.AddDays(-10);
            FieldDataChangedEndDate = DateTime.Now;
        }

        public DateTime FieldDataChangedStartDate { get; set; }

        public DateTime FieldDataChangedEndDate { get; set; }
    }
}