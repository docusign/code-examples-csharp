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
        protected DSConfiguration DSConfig { get; }

        private IConfiguration Configuration { get; }
        public LauncherTexts(DSConfiguration dsconfiguration, IConfiguration configuration)
        {
            DSConfig = dsconfiguration;
            Configuration = configuration;
        }

        private ManifestStructure manifestStructure;

        public ManifestStructure ManifestStructure
        {
            get {
                if (manifestStructure == null)
                {
                    manifestStructure = SetupManifestData(GetTextManifestDependingOnCurrentAPI());
                }
                return manifestStructure;
            }
        }

        private string GetTextManifestDependingOnCurrentAPI()
        {
            string linkToManifest = string.Empty;

            if (Configuration["ExamplesAPI"] == ExamplesAPIType.ESignature.ToString() || DSConfig.QuickACG == "true")
            {
                linkToManifest = DSConfig.ESignatureManifest;
            }
            else if (Configuration["ExamplesAPI"] == ExamplesAPIType.Click.ToString())
            {
                linkToManifest = DSConfig.ClickManifest;
            }
            else if (Configuration["ExamplesAPI"] == ExamplesAPIType.Rooms.ToString())
            {
                linkToManifest = DSConfig.RoomsManifest;
            }
            else if (Configuration["ExamplesAPI"] == ExamplesAPIType.Monitor.ToString())
            {
                linkToManifest = DSConfig.MonitorManifest;
            }
            else if (Configuration["ExamplesAPI"] == ExamplesAPIType.Admin.ToString())
            {
                linkToManifest = DSConfig.AdminManifest;
            }

            return linkToManifest;
        }

        private ManifestStructure SetupManifestData(string fileName)
        {
            return JsonConvert.DeserializeObject<ManifestStructure>(
                    LoadFileContentAsync(fileName).Result,
                    new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        private async Task<string> LoadFileContentAsync(string fileDownloadLink)
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
                throw new Exception(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        "Could not load file: {0}.", 
                        fileDownloadLink
                    ), 
                    exception);
            }
        }
    }
}