using DocuSign.CodeExamples.Models;

namespace DocuSign.CodeExamples
{
    public interface IRequestItemsService
    {
        public string EgName { get; set; }

        public Session Session { get; set; }

        public User User { get; set; }

        public string EnvelopeId { get; set; }

        public string Status { get; set; }

        public void Logout();

        public bool CheckToken(int bufferMin);
    }
}