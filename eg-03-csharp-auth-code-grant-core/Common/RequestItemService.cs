using DocuSign.eSign.Client;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace eg_03_csharp_auth_code_grant_core.Common
{
    public class RequestItemsService : IRequestItemsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;
        private readonly string _id;
        private string API_CLIENT_KEY = "{0}_ApiClient";
        private string CONFIG_KEY = "{0}_DocusignConfig";
        private string BASE_URI = "https://demo.docusign.net/restapi";
        private string _accessToken;        

        public RequestItemsService(IHttpContextAccessor httpContextAccessor, IMemoryCache cache)
        {
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            Status = "sent";
            var identity = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null && identity.IsAuthenticated)
            {
                _accessToken = identity.FindFirst(x => x.Type.Equals("access_token")).Value;
                _id = httpContextAccessor.HttpContext.User.Identity.Name;
            }
        }

        public ApiClient DefaultApiClient
        {
            get
            {
                var key = string.Format(API_CLIENT_KEY, _id);
                ApiClient apiClient = _cache.Get<ApiClient>("apiClient");
                if (apiClient == null)
                {
                    apiClient = new ApiClient(BASE_URI);
                    _cache.Set(key, apiClient);
                }

                return apiClient;
            }
        }

        public Configuration DefaultConfiguration
        {
            get
            {
                var key = string.Format(CONFIG_KEY, _id);
                var docuSignConfig = _cache.Get<Configuration>(key);

                if (docuSignConfig == null)
                {
                    docuSignConfig = new Configuration(new ApiClient(BASE_URI));
                    _cache.Set(key, docuSignConfig);
                }
                docuSignConfig.AddDefaultHeader("Authorization", "Bearer " + _accessToken);
                return docuSignConfig;
            }
        }
        private string GetKey(string key)
        {
            return string.Format("{0}_{1}", _id, key);
        }
        public string EgName {
            get => _cache.Get<string>(GetKey("EgName"));
            set => _cache.Set(GetKey("EgName"), value);
        }

        public Session Session {
            get => _cache.Get<Session>(GetKey("Session"));
            set => _cache.Set(GetKey("Session"), value);
        }

        public User User {
            get => _cache.Get<User>(GetKey("User"));
            set => _cache.Set(GetKey("User"), value);
        }

        public string EnvelopeId {
            get => _cache.Get<string>(GetKey("EnvelopeId"));
            set => _cache.Set(GetKey("EnvelopeId"), value);
        }

        public string DocumentId {
            get => _cache.Get<string>(GetKey("DocumentId"));
            set => _cache.Set(GetKey("DocumentId"), value);
        }

        public EnvelopeDocuments EnvelopeDocuments {
            get => _cache.Get<EnvelopeDocuments>(GetKey("EnvelopeDocuments"));
            set => _cache.Set(GetKey("EnvelopeDocuments"), value);
        }

        public string TemplateId {
            get => _cache.Get<string>(GetKey("TemplateId"));
            set => _cache.Set(GetKey("TemplateId"), value);
        }

        public string Status { get; set; }
    }
}
