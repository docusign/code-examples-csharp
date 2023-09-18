// <copyright file="Locals.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Locals
    {
        public DsConfiguration DsConfig { get; set; }

        public User User { get; set; }

        public Session Session { get; set; }

        public string Messages { get; set; }

        public object Json { get; internal set; }
    }
}
