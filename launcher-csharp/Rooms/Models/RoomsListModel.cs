// <copyright file="RoomsListModel.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Models
{
    using System.Collections.Generic;
    using DocuSign.Rooms.Model;

    public class RoomsListModel
    {
        public int RoomId { get; set; }

        public List<RoomSummary> Rooms { get; set; }
    }
}