using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using DocuSign.Click.Model;
using DocuSign.eSign.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace launcher_csharp.Tests
{
    public sealed class TestConfig : ITestConfig
    {
        public string ClientId { get; set; }

        public string Host { get; set; }

        public DocuSignClient ApiClient { get; set; }

        public string AccountId { get; set; }

        public string SignerEmail { get; set; }

        public string SignerName { get; set; }

        public string ImpersonatedUserId { get; set; }

        public string OAuthBasePath { get; set; }

        public string PrivateKey { get; set; }

        public string AccessToken { get; set; }

        public string BasePath { get; set; }

        public string TemplateId { get; set; }

        public string PathToSolution { get; set; }

        public string BrandId { get; set; }

        public ClickwrapVersionSummaryResponse InactiveClickwrap { get; set; }

        private static readonly Lazy<TestConfig> TestConfigLazy =
            new Lazy<TestConfig>(() => new TestConfig());

        public static TestConfig Instance => TestConfigLazy.Value;

        public TestConfig()
        {
            this.PathToSolution = @"../../../../launcher-csharp/";
            this.Host = "https://demo.docusign.net/restapi";
            this.OAuthBasePath = "account-d.docusign.com";
            this.PrivateKey = PathToSolution + "private.key";

            string keysFile = File.ReadAllText(this.PathToSolution + "appsettings.json");
            var keysFileJObject = (JObject)JsonConvert.DeserializeObject(keysFile);

            this.ImpersonatedUserId = keysFileJObject["DocuSignJWT"]["ImpersonatedUserId"].Value<string>();
            this.ClientId = keysFileJObject["DocuSignJWT"]["ClientId"].Value<string>();
            this.SignerEmail = keysFileJObject["DocuSign"]["SignerEmail"].Value<string>();
            this.SignerName = keysFileJObject["DocuSign"]["SignerName"].Value<string>();
        }

        public void OpenUrlUsingConsoleWindow(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")
                {
                    CreateNoWindow = false
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }
    }
}
