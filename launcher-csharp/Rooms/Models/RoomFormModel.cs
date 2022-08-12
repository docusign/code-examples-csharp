// <copyright file="RoomFormModel.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Models
{
    using System;
    using System.Collections.Generic;
    using DocuSign.Rooms.Model;

    public class RoomFormModel
    {
        public int RoomId { get; set; }

        public List<RoomSummary> Rooms { get; set; }

        public Guid FormId { get; set; }

        public List<FormSummary> Forms { get; set; }
    }
}