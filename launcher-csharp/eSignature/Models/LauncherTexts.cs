// <copyright file="LauncherTexts.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Models
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.ESignature.Models;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class LauncherTexts
    {
        public LauncherTexts(DSConfiguration dsconfiguration, IConfiguration configuration)
        {
            this.DSConfig = dsconfiguration;
            this.Configuration = configuration;
        }

        protected DSConfiguration DSConfig { get; }

        private IConfiguration Configuration { get; }

        private ManifestStructure manifestStructure;

        public ManifestStructure ManifestStructure
        {
            get
            {
                if (this.manifestStructure == null)
                {
                    this.manifestStructure = this.SetupManifestData(this.GetTextManifestDependingOnCurrentAPI());
                }

                return this.manifestStructure;
            }
        }

        private string GetTextManifestDependingOnCurrentAPI()
        {
            string linkToManifest = string.Empty;

            if (this.Configuration["ExamplesAPI"] == ExamplesAPIType.ESignature.ToString() || this.DSConfig.QuickACG == "true")
            {
                linkToManifest = this.DSConfig.ESignatureManifest;
            }
            else if (this.Configuration["ExamplesAPI"] == ExamplesAPIType.Click.ToString())
            {
                linkToManifest = this.DSConfig.ClickManifest;
            }
            else if (this.Configuration["ExamplesAPI"] == ExamplesAPIType.Rooms.ToString())
            {
                linkToManifest = this.DSConfig.RoomsManifest;
            }
            else if (this.Configuration["ExamplesAPI"] == ExamplesAPIType.Monitor.ToString())
            {
                linkToManifest = this.DSConfig.MonitorManifest;
            }
            else if (this.Configuration["ExamplesAPI"] == ExamplesAPIType.Admin.ToString())
            {
                linkToManifest = this.DSConfig.AdminManifest;
            }

            return linkToManifest;
        }

        private ManifestStructure SetupManifestData(string fileName)
        {
            return JsonConvert.DeserializeObject<ManifestStructure>(
                    this.LoadFileContentAsync(fileName).Result,
                    new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        private async Task<string> LoadFileContentAsync(string fileDownloadLink)
        {
            HttpClient httpClient = new HttpClient();

            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, fileDownloadLink))
                using (var resoponse = await httpClient.SendAsync(request, CancellationToken.None).ConfigureAwait(false))
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
                        fileDownloadLink),
                    exception);
            }
        }
    }
}