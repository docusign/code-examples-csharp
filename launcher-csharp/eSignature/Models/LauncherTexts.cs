using Microsoft.Extensions.Configuration;
using DocuSign.CodeExamples.Common;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DocuSign.CodeExamples.eSignature.Models;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Models
{
    public class LauncherTexts
    {
        protected DSConfiguration Config { get; }

        private IConfiguration _configuration { get; }
        public LauncherTexts(DSConfiguration dsconfiguration, IConfiguration configuration)
        {
            Config = dsconfiguration;
            _configuration = configuration;
        }

        private ManifestStructure manifestStructure;

        public ManifestStructure ManifestStructure
        {
            get {
                if (manifestStructure == null)
                {
                    manifestStructure = SetupManifestData(Config.ESignatureManifest);
                }
                return manifestStructure;
            }
        }

        public ManifestStructure SetupManifestData(string fileName)
        {
            var a = LoadFileContentAsync(fileName).Result;
            return JsonConvert.DeserializeObject<ManifestStructure>(
                    LoadFileContentAsync(fileName).Result,
                    new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        public async Task<string> LoadFileContentAsync(string fileDownloadLink)
        {
            HttpClient _httpClient = new HttpClient();

            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, fileDownloadLink))
                using (var resoponse = await _httpClient.SendAsync(request, CancellationToken.None).ConfigureAwait(false))
                {
                    if (resoponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return await resoponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }

                    throw new Exception(await resoponse.Content.ReadAsStringAsync().ConfigureAwait(false));
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "Could not load file: {0}.", fileDownloadLink), exception);
            }
        }
    }
}