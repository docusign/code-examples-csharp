using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using eg_03_csharp_auth_code_grant_core.Models;
using System;

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
        public void UpdateUserFromJWT();
        public void Logout();
        public bool CheckToken(int bufferMin);
    }
}