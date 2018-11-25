using System;

namespace eg_03_csharp_auth_code_grant_core.Models
{
    public class User
    {
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? ExpireIn { get; set; }
    }
}