using System;

namespace eg_03_csharp_auth_code_grant_core.Rooms.Models
{
    public class RoomFilterModel
    {
        public RoomFilterModel()
        {
            FieldDataChangedStartDate = DateTimeOffset.Now.AddDays(-10);
            FieldDataChangedEndDate = DateTimeOffset.Now;
        }

        public DateTimeOffset FieldDataChangedStartDate { get; set; }

        public DateTimeOffset FieldDataChangedEndDate { get; set; }
    }
}