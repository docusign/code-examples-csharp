using DocuSign.CodeExamples.Models;

namespace DocuSign.CodeExamples
{
    public interface IRequestItemsService
    {        
        string EgName { get; set; }

        Session Session { get; set; }

        User User { get; set; }

        string EnvelopeId { get; set; }

        string Status { get; set; }

        public void Logout();

        public bool CheckToken(int bufferMin);
    }
}