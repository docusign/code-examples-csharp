using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eg_03_csharp_auth_code_grant_core.Models
{
    public class Locals
    {
        public DSConfiguration DsConfig { get; set; }
        public User User { get; set; }
        public Session Session { get; set; }
        public String Messages { get; set; }
        public object Json { get; internal set; }
    }
}
