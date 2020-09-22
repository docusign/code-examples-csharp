using System;

namespace DocuSign.CodeExamples.Models
{
    public class User
    {
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? ExpireIn { get; set; }
        public string AccountId { get; set; }
    }
}