using System;

namespace DocuSign.CodeExamples.Rooms.Models
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