using DocuSign.eSign.Client;
using System;
using System.Linq;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using static DocuSign.eSign.Client.Auth.OAuth;
using static DocuSign.eSign.Client.Auth.OAuth.UserInfo;

namespace DocuSign.CodeExamples.Common
{
    public class RequestItemsService : IRequestItemsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly string _id;
        private OAuthToken _authToken;
        protected static ApiClient _apiClient { get; private set; }
        private static Account _account { get; set; }
        private static Guid? _organizationId { get; set; }

        public RequestItemsService(IHttpContextAccessor httpContextAccessor, IMemoryCache cache, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _cache = cache;
            Status = "sent";
            _apiClient ??= new ApiClient();
            var identity = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null && identity.IsAuthenticated)
            {
                _id = httpContextAccessor.HttpContext.User.Identity.Name;
            }
        }

        public void UpdateUserFromJWT()
        {
            _authToken = Authentication.JWTAuth.AuthenticateWithJWT(_configuration["ExamplesAPI"], _configuration["DocuSignJWT:ClientId"], _configuration["DocuSignJWT:ImpersonatedUserId"],
                                                        _configuration["DocuSignJWT:AuthServer"], _configuration["DocuSignJWT:PrivateKeyFile"]);
            _account = GetAccountInfo(_authToken);

            this.User = new User
            {
                Name = _account.AccountName,
                AccessToken = _authToken.access_token,
                ExpireIn = DateTime.Now.AddSeconds(_authToken.expires_in.Value),
                AccountId = _account.AccountId
            };

            this.Session = new Session
            {
                AccountId = _account.AccountId,
                AccountName = _account.AccountName,
                BasePath = _account.BaseUri,
                RoomsApiBasePath = _configuration["DocuSign:RoomsApiEndpoint"],
                AdminApiBasePath = _configuration["DocuSign:AdminApiEndpoint"]
            };
        }

        public void Logout()
        {
            this._authToken = null;
            this.EgName = null;
            this.User = null;
        }

        public bool CheckToken(int bufferMin = 60)
        {
            bool isAuthCodeGrantAuthenticated = this._httpContextAccessor.HttpContext.User.Identity.IsAuthenticated
                && (DateTime.Now.Subtract(TimeSpan.FromMinutes(bufferMin)) < User.ExpireIn.Value);

            bool isJWTGrantAuthenticated = User?.AccessToken != null
                    && (DateTime.Now.Subtract(TimeSpan.FromMinutes(bufferMin)) < User.ExpireIn.Value);

            return isAuthCodeGrantAuthenticated || isJWTGrantAuthenticated;
        }

        private string GetKey(string key)
        {
            return string.Format("{0}_{1}", _id, key);
        }

        public string EgName
        {
            get => _cache.Get<string>(GetKey("EgName"));
            set => _cache.Set(GetKey("EgName"), value);
        }

        public Session Session
        {
            get => _cache.Get<Session>(GetKey("Session"));
            set => _cache.Set(GetKey("Session"), value);
        }

        public User User
        {
            get => _cache.Get<User>(GetKey("User"));
            set => _cache.Set(GetKey("User"), value);
        }
        public Guid? OrganizationId
        {
            get
            {
                if (_organizationId == null)
                {
                    var apiClient = new DocuSign.Admin.Client.ApiClient(this.Session.AdminApiBasePath);
                    apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + this.User.AccessToken);
                    var accountApi = new DocuSign.Admin.Api.AccountsApi(apiClient);
                    var org = accountApi.GetOrganizations().Organizations.FirstOrDefault();
                    if (org == null)
                    {
                        throw new DocuSign.Admin.Client.ApiException(0, "You must create an organization for this account to be able to use the DocuSign Admin API." );
                    }
                        else
                    {
                        _organizationId = org.Id;
                    }
                }
                return _organizationId;
            }
            set
            {
                _organizationId = value;
            }
        }

        public string EnvelopeId
        {
            get => _cache.Get<string>(GetKey("EnvelopeId"));
            set => _cache.Set(GetKey("EnvelopeId"), value);
        }

        public string DocumentId
        {
            get => _cache.Get<string>(GetKey("DocumentId"));
            set => _cache.Set(GetKey("DocumentId"), value);
        }

        public EnvelopeDocuments EnvelopeDocuments
        {
            get => _cache.Get<EnvelopeDocuments>(GetKey("EnvelopeDocuments"));
            set => _cache.Set(GetKey("EnvelopeDocuments"), value);
        }

        public string TemplateId
        {
            get => _cache.Get<string>(GetKey("TemplateId"));
            set => _cache.Set(GetKey("TemplateId"), value);
        }

        public string ClickwrapId
        {
            get => _cache.Get<string>(GetKey("ClickwrapId"));
            set => _cache.Set(GetKey("ClickwrapId"), value);
        }

        public string ClickwrapName
        {
            get => _cache.Get<string>(GetKey("ClickwrapName"));
            set => _cache.Set(GetKey("ClickwrapName"), value);
        }

        public string PausedEnvelopeId
        {
            get => _cache.Get<string>(GetKey("PausedEnvelopeId"));
            set => _cache.Set(GetKey("PausedEnvelopeId"), value);
        }

        public string Status { get; set; }

        private Account GetAccountInfo(OAuthToken authToken)
        {
            _apiClient.SetOAuthBasePath(this._configuration["DocuSignJWT:AuthServer"]);
            UserInfo userInfo = _apiClient.GetUserInfo(authToken.access_token);
            Account acct = userInfo.Accounts.FirstOrDefault();
            if (acct == null)
            {
                throw new Exception("The user does not have access to account");
            }

            return acct;
        }
    }
}
