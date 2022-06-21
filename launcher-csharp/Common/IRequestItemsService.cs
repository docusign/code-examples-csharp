using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using DocuSign.CodeExamples.Models;
using System;

namespace DocuSign.CodeExamples
{
    public interface IRequestItemsService
    {        
        public string EgName { get; set; }

        public Session Session { get; set; }

        public User User { get; set; }
        public Guid? OrganizationId { get; set; }
        string EnvelopeId { get; set; }
        public string DocumentId { get; set; }
        public string ClickwrapId { get; set; }
        public string ClickwrapName { get; set; }

        public EnvelopeDocuments EnvelopeDocuments { get; set; }
        public string TemplateId { get; set; }
        public string PausedEnvelopeId { get; set; }
        public string Status { get; set; }

        public string EmailAddress { get; set; }

        public void UpdateUserFromJWT();
        public void Logout();
        public bool CheckToken(int bufferMin);
    }
}