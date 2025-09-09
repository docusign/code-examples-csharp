// <copyright file="RoomModel.cs" company="Docusign">
// Copyright (c) Docusign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Rooms.Models
{
    using System.Collections.Generic;
    using DocuSign.Rooms.Model;

    public class RoomModel
    {
        public string Name { get; set; }

        public int TemplateId { get; set; }

        public List<RoomTemplate> Templates { get; set; }
    }
}
