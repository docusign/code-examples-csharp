using DocuSign.eSign.Client;

namespace launcher_csharp.Tests
{
    public interface ITestConfig
    {
        public string ClientId { get; set; }

        public string Host { get; set; }

        public DocuSignClient ApiClient { get; set; }

        public string AccountId { get; set; }

        public string SignerEmail { get; set; }

        public string SignerName { get; set; }

        public string AccessToken { get; set; }

        public string BasePath { get; set; }

        public string TemplateId { get; set; }

        public string ImpersonatedUserId { get; set; }

        public string OAuthBasePath { get; set; }

        public string PrivateKey { get; set; }

        public string PathToSolution { get; set; }

        public byte[] PrivateKeyBytes { get; set; }

        public void OpenUrlUsingConsoleWindow(string url);
    }
}