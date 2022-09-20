// <copyright file="RoomFilterModel.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Models
{
    using System;

    public class RoomFilterModel
    {
        public RoomFilterModel()
        {
            this.FieldDataChangedStartDate = DateTimeOffset.Now.AddDays(-10);
            this.FieldDataChangedEndDate = DateTimeOffset.Now;
        }

        public DateTimeOffset FieldDataChangedStartDate { get; set; }

        public DateTimeOffset FieldDataChangedEndDate { get; set; }
    }
}