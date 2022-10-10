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
        protected DSConfiguration DSConfig { get; }

        public LauncherTexts(DSConfiguration dsconfiguration)
        {
            this.DSConfig = dsconfiguration;
        }

        private ManifestStructure manifestStructure;

        public ManifestStructure ManifestStructure
        {
            get
            {
                if (this.manifestStructure == null)
                {
                    this.manifestStructure = this.SetupManifestData(this.DSConfig.CodeExamplesManifest);
                }

                return this.manifestStructure;
            }
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