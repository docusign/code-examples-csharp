using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.ESignature.Models;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Views;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DocuSign.CodeExamples.Controllers
{
    public abstract class EgController : Controller
    {
        public abstract string EgName { get; }

        protected CodeExampleText CodeExampleText { get; set; }
        public DsConfiguration Config { get; }
        public LauncherTexts LauncherTexts { get; }
        public IRequestItemsService RequestItemsService { get; }

        public EgController(DsConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
        {
            Config = config;
            RequestItemsService = requestItemsService;
            ViewBag.csrfToken = "";
            LauncherTexts = launcherTexts;
        }

        [Route("/")]
        public virtual IActionResult Index()
        {
            var identity = this.Request.HttpContext.User.Identity as ClaimsIdentity;
            if (identity.IsAuthenticated == true)
            {
                RequestItemsService.User = RequestItemsService.User ?? new User
                {
                    Name = identity.FindFirst(x => x.Type.Equals(ClaimTypes.Name)).Value,
                    AccessToken = identity.FindFirst(x => x.Type.Equals("access_token")).Value,
                    RefreshToken = identity.FindFirst(x => x.Type.Equals("refresh_token"))?.Value,
                    ExpireIn = DateTime.Parse(identity.FindFirst(x => x.Type.Equals("expires_in")).Value)
                };
                RequestItemsService.Session = RequestItemsService.Session ?? new Session
                {
                    AccountId = identity.FindFirst(x => x.Type.Equals("account_id")).Value,
                    AccountName = identity.FindFirst(x => x.Type.Equals("account_name")).Value,
                    BasePath = identity.FindFirst(x => x.Type.Equals("base_uri")).Value
                };
            }

            return Redirect("eg001");
        }

        [Route("/dsReturn")]
        public IActionResult DsReturn()
        {
            return Redirect("eg001");
        }

        [HttpGet]
        public virtual IActionResult Get()
        {
            // Check that the token is valid and will remain valid for awhile to enable the
            // user to fill out the form. If the token is not available, now is the time
            // to have the user authenticate or re-authenticate.
            bool tokenOk = CheckToken();

            if (tokenOk)
            {
                ViewBag.envelopeOk = RequestItemsService.EnvelopeId != null;
                ViewBag.gatewayOk = Config.GatewayAccountId != null && Config.GatewayAccountId.Length > 25;
                ViewBag.source = CreateSourcePath();
                ViewBag.documentation = Config.Documentation + EgName;
                ViewBag.showDoc = Config.Documentation != null;
                ViewBag.User = RequestItemsService.User;
                ViewBag.Session = RequestItemsService.Session;
                ViewBag.DsConfig = Config;
                ViewBag.User = RequestItemsService.User;
                ViewBag.DsConfig = Config;
                InitializeInternal();

                if (Config.QuickAcg == "true" && !(this is EmbeddedSigningCeremony))
                {
                    return Redirect("eg001");
                }

                return View(this.EgName, this);
            }

            return Redirect("/ds/mustAuthenticate");
        }

        protected virtual void InitializeInternal()
        {
            this.ViewBag.CodeExampleText = this.CodeExampleText;
            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
        }

        public dynamic CreateSourcePath()
        {
            var uri = Config.GithubExampleUrl;
            uri = $"{uri}/eSignature";
            return $"{uri}/Controllers/{this.GetType().Name}.cs";
        }

        protected bool CheckToken(int bufferMin = 60)
        {
            return RequestItemsService.CheckToken(bufferMin);
        }

        protected CodeExampleText GetExampleText(string exampleName, ExamplesApiType examplesApiType)
        {
            int exampleNumber = int.Parse(Regex.Match(exampleName, @"\d+").Value);
            var groups = this.LauncherTexts.ManifestStructure.ApIs
                .Find(x => x.Name.ToLowerInvariant() == examplesApiType.ToString().ToLowerInvariant())
                .Groups;

            foreach (var group in groups)
            {
                var example = group.Examples.Find((example) => example.ExampleNumber == exampleNumber);

                if (example != null)
                {
                    return example;
                }
            }

            return null;
        }
    }
}