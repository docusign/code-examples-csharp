using DocuSign.eSign.Client;
using System;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace DocuSign.CodeExamples.Common
{
    public class RequestItemsService : IRequestItemsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;
        private readonly string _id;
        protected static ApiClient _apiClient { get; private set; }

        public RequestItemsService(IHttpContextAccessor httpContextAccessor, IMemoryCache cache, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            Status = "sent";
            _apiClient ??= new ApiClient();
            var identity = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null && identity.IsAuthenticated)
            {
                _id = httpContextAccessor.HttpContext.User.Identity.Name;
            }
        }

        public void Logout()
        {
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

        public string EnvelopeId
        {
            get => _cache.Get<string>(GetKey("EnvelopeId"));
            set => _cache.Set(GetKey("EnvelopeId"), value);
        }

        public string Status { get; set; }       
    }
}
