using DocuSign.eSign.Client;
using eg_03_csharp_auth_code_grant_core.Models;

namespace eg_03_csharp_auth_code_grant_core
{
    public interface IRequestItemsService
    {        
        string EgName { get; set; }

        Session Session { get; set; }

        User User { get; set; }
        string EnvelopeId { get; set; }
        string DocumentId { get; set; }
        EnvelopeDocuments EnvelopeDocuments { get; set; }
        string TemplateId { get; set; }
        string Status { get; set; }
    }
}