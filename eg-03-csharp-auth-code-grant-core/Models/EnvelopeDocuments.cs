using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eg_03_csharp_auth_code_grant_core.Models
{
    public class EnvelopeDocuments
    {
        public string EnvelopeId { get; set; }
        public List<EnvelopeDocItem> Documents { get; set; }
    }
}
