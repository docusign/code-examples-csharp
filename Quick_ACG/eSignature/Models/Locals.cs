using System;

namespace DocuSign.CodeExamples.Models
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
