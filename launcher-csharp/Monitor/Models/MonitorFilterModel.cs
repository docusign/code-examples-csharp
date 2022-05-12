using System;

namespace DocuSign.CodeExamples.Monitor.Models
{
    public class MonitorFilterModel
    {
        public MonitorFilterModel()
        {
            FieldDataChangedStartDate = DateTimeOffset.Now.AddDays(-10);
            FieldDataChangedEndDate = DateTimeOffset.Now;
        }

        public DateTimeOffset FieldDataChangedStartDate { get; set; }

        public DateTimeOffset FieldDataChangedEndDate { get; set; }
    }
}
