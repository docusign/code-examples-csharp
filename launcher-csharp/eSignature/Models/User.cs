// <copyright file="User.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Models
{
    using System;

    public class User
    {
        public string Name { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime? ExpireIn { get; set; }

        public string AccountId { get; set; }
    }
}