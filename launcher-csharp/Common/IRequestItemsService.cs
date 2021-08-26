using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using DocuSign.CodeExamples.Models;
using System;

namespace DocuSign.CodeExamples
{
    public interface IRequestItemsService
    {        
        string EgName { get; set; }

        Session Session { get; set; }

        User User { get; set; }
        public Guid? OrganizationId { get; set; }
        string EnvelopeId { get; set; }
        string DocumentId { get; set; }
        string ClickwrapId { get; set; }
        EnvelopeDocuments EnvelopeDocuments { get; set; }
        string TemplateId { get; set; }
        string PausedEnvelopeId { get; set; }
        string Status { get; set; }

        public void UpdateUserFromJWT();
        public void Logout();
        public bool CheckToken(int bufferMin);
    }
}