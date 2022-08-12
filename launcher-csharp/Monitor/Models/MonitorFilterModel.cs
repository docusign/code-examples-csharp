// <copyright file="MonitorFilterModel.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Monitor.Models
{
    using System;

    public class MonitorFilterModel
    {
        public MonitorFilterModel()
        {
            this.FieldDataChangedStartDate = DateTimeOffset.Now.AddDays(-10);
            this.FieldDataChangedEndDate = DateTimeOffset.Now;
        }

        public DateTimeOffset FieldDataChangedStartDate { get; set; }

        public DateTimeOffset FieldDataChangedEndDate { get; set; }
    }
}
