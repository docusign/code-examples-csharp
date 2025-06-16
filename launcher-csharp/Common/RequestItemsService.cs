// <copyright file="RequestItemsService.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Client.Auth;
    using Docusign.IAM.SDK.Models.Components;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using static DocuSign.eSign.Client.Auth.OAuth;

    public class RequestItemsService : IRequestItemsService
    {
        private static OAuth.UserInfo.Account account;

        private static Guid? organizationId;

        #nullable enable
        private static string? authenticatedUserEmail;
        #nullable disable

        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly IMemoryCache cache;

        #nullable enable
        private readonly string? id;
        #nullable disable

        private OAuthToken authToken;

        public RequestItemsService(IHttpContextAccessor httpContextAccessor, IMemoryCache cache, IConfiguration configuration)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.Configuration = configuration;
            this.cache = cache;
            this.Status = "sent";
            DocuSignClient ??= new DocuSignClient();
            var identity = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null && identity.IsAuthenticated)
            {
                this.id = httpContextAccessor?.HttpContext.User.Identity.Name;
            }
        }

        public IConfiguration Configuration { get; set; }

        #nullable enable
        public string? EgName
        {
            get => this.cache.Get<string>(this.GetKey("EgName"));
            set => this.cache.Set(this.GetKey("EgName"), value);
        }
        #nullable disable

        public Session Session
        {
            get => this.cache.Get<Session>(this.GetKey("Session"));
            set => this.cache.Set(this.GetKey("Session"), value);
        }

        #nullable enable
        public User? User
        {
            get => this.cache.Get<User>(this.GetKey("User"));
            set => this.cache.Set(this.GetKey("User"), value);
        }
        #nullable disable

        public Guid? OrganizationId
        {
            get
            {
                if (organizationId == null)
                {
                    var apiClient = new DocuSign.Admin.Client.DocuSignClient(this.Session.AdminApiBasePath);
                    apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + this.User.AccessToken);
                    var accountApi = new DocuSign.Admin.Api.AccountsApi(apiClient);
                    var org = accountApi.GetOrganizations().Organizations.FirstOrDefault();
                    if (org == null)
                    {
                        throw new DocuSign.Admin.Client.ApiException(0, "You must create an organization for this account to be able to use the DocuSign Admin API.");
                    }
                    else
                    {
                        organizationId = org.Id;
                    }
                }

                return organizationId;
            }

            set
            {
                organizationId = value;
            }
        }

        public string AuthenticatedUserEmail
        {
            get
            {
                if (authenticatedUserEmail == null)
                {
                    DocuSignClient.SetOAuthBasePath(this.Configuration["DocuSignJWT:AuthServer"]);
                    OAuth.UserInfo userInfo = DocuSignClient.GetUserInfo(this.User?.AccessToken);

                    authenticatedUserEmail = userInfo.Email;
                }

                return authenticatedUserEmail;
            }

            set
            {
                authenticatedUserEmail = value;
            }
        }

        public string EnvelopeId
        {
            get => this.cache.Get<string>(this.GetKey("EnvelopeId"));
            set => this.cache.Set(this.GetKey("EnvelopeId"), value);
        }

        public List<TabInfo> ExtensionApps
        {
            get => this.cache.Get<List<TabInfo>>(this.GetKey("ExtensionApps"));
            set => this.cache.Set(this.GetKey("ExtensionApps"), value);
        }

        public string DocumentId
        {
            get => this.cache.Get<string>(this.GetKey("DocumentId"));
            set => this.cache.Set(this.GetKey("DocumentId"), value);
        }

        public EnvelopeDocuments EnvelopeDocuments
        {
            get => this.cache.Get<EnvelopeDocuments>(this.GetKey("EnvelopeDocuments"));
            set => this.cache.Set(this.GetKey("EnvelopeDocuments"), value);
        }

        public string TemplateId
        {
            get => this.cache.Get<string>(this.GetKey("TemplateId"));
            set => this.cache.Set(this.GetKey("TemplateId"), value);
        }

        public string WorkflowId
        {
            get => this.cache.Get<string>(this.GetKey("WorkflowId"));
            set => this.cache.Set(this.GetKey("WorkflowId"), value);
        }

        public bool IsWorkflowPublished
        {
            get => this.cache.Get<bool>(this.GetKey("WorkflowPublished"));
            set => this.cache.Set(this.GetKey("WorkflowPublished"), value);
        }

        public string InstanceId
        {
            get => this.cache.Get<string>(this.GetKey("InstanceId"));
            set => this.cache.Set(this.GetKey("InstanceId"), value);
        }

        public string WebFormsTemplateId
        {
            get => this.cache.Get<string>(this.GetKey("WebFormsTemplateId"));
            set => this.cache.Set(this.GetKey("WebFormsTemplateId"), value);
        }

        public string ClickwrapId
        {
            get => this.cache.Get<string>(this.GetKey("ClickwrapId"));
            set => this.cache.Set(this.GetKey("ClickwrapId"), value);
        }

        public string ClickwrapName
        {
            get => this.cache.Get<string>(this.GetKey("ClickwrapName"));
            set => this.cache.Set(this.GetKey("ClickwrapName"), value);
        }

        public string PausedEnvelopeId
        {
            get => this.cache.Get<string>(this.GetKey("PausedEnvelopeId"));
            set => this.cache.Set(this.GetKey("PausedEnvelopeId"), value);
        }

        public string Status { get; set; }

        public string EmailAddress
        {
            get => this.cache.Get<string>(this.GetKey("EmailAddress"));
            set => this.cache.Set(this.GetKey("EmailAddress"), value);
        }

        protected static DocuSignClient DocuSignClient { get; private set; }

        public void UpdateUserFromJwt()
        {
            this.authToken = Authentication.JwtAuth.AuthenticateWithJwt(
                this.Configuration["API"],
                this.Configuration["DocuSignJWT:ClientId"],
                this.Configuration["DocuSignJWT:ImpersonatedUserId"],
                this.Configuration["DocuSignJWT:AuthServer"],
                DsHelper.ReadFileContent(this.Configuration["DocuSignJWT:PrivateKeyFile"]));
            account = this.GetAccountInfo(this.authToken);

            this.User = new User
            {
                Name = account.AccountName,
                AccessToken = this.authToken.access_token,
                ExpireIn = DateTime.Now,
                AccountId = account.AccountId,
            };

            if (this.authToken.expires_in.HasValue)
            {
                this.User.ExpireIn.Value.AddSeconds(this.authToken.expires_in.Value);
            }

            this.Session = new Session
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                BasePath = account.BaseUri,
                RoomsApiBasePath = this.Configuration["DocuSign:RoomsApiEndpoint"],
                AdminApiBasePath = this.Configuration["DocuSign:AdminApiEndpoint"],
                WebFormsBasePath = this.Configuration["DocuSign:WebFormsBasePath"],
                MaestroApiBasePath = this.Configuration["DocuSign:MaestroApiEndpoint"],
            };
        }

        public void Logout()
        {
            this.authToken = null;
            this.EgName = null;
            this.User = null;
        }

        public bool CheckToken(int bufferMin = 60)
        {
            bool isAuthCodeGrantAuthenticated = this.httpContextAccessor.HttpContext.User.Identity.IsAuthenticated
                && (DateTime.Now.Subtract(TimeSpan.FromMinutes(bufferMin)) < this.User.ExpireIn.Value);

            bool isJwtGrantAuthenticated = this.User?.AccessToken != null
                    && (DateTime.Now.Subtract(TimeSpan.FromMinutes(bufferMin)) < this.User.ExpireIn.Value);

            return isAuthCodeGrantAuthenticated || isJwtGrantAuthenticated;
        }

        public string IdentifyApiOfCodeExample(string eg)
        {
            if (string.IsNullOrEmpty(eg))
            {
                return ExamplesApiType.ESignature.ToString();
            }

            var currentApiType = string.Empty;
            if (eg.Contains(ExamplesApiType.Rooms.ToKeywordString()))
            {
                currentApiType = ExamplesApiType.Rooms.ToString();
            }
            else if (eg.Contains(ExamplesApiType.Click.ToKeywordString()))
            {
                currentApiType = ExamplesApiType.Click.ToString();
            }
            else if (eg.Contains(ExamplesApiType.Monitor.ToKeywordString()))
            {
                currentApiType = ExamplesApiType.Monitor.ToString();
            }
            else if (eg.Contains(ExamplesApiType.Admin.ToKeywordString()))
            {
                currentApiType = ExamplesApiType.Admin.ToString();
            }
            else if (eg.Contains(ExamplesApiType.Connect.ToKeywordString()))
            {
                currentApiType = ExamplesApiType.Connect.ToString();
            }
            else if (eg.Contains(ExamplesApiType.Maestro.ToKeywordString()))
            {
                currentApiType = ExamplesApiType.Maestro.ToString();
            }
            else if (eg.Contains(ExamplesApiType.WebForms.ToKeywordString()))
            {
                currentApiType = ExamplesApiType.WebForms.ToString();
            }
            else if (eg.Contains(ExamplesApiType.Notary.ToKeywordString()))
            {
                currentApiType = ExamplesApiType.Notary.ToString();
            }
            else if (eg.Contains(ExamplesApiType.ConnectedFields.ToKeywordString()))
            {
                currentApiType = ExamplesApiType.ConnectedFields.ToString();
            }
            else if (eg.Contains(ExamplesApiType.Navigator.ToKeywordString()))
            {
                currentApiType = ExamplesApiType.Navigator.ToString();
            }
            else
            {
                currentApiType = ExamplesApiType.ESignature.ToString();
            }

            return currentApiType;
        }

        private string GetKey(string key)
        {
            return string.Format("{0}_{1}", this.id, key);
        }

        private OAuth.UserInfo.Account GetAccountInfo(OAuthToken authToken)
        {
            DocuSignClient.SetOAuthBasePath(this.Configuration["DocuSignJWT:AuthServer"]);
            OAuth.UserInfo userInfo = DocuSignClient.GetUserInfo(authToken.access_token);
            var accounts = userInfo.Accounts;

            var targetAccountIdString = this.Configuration["DocuSign:TargetAccountId"];

            if (Guid.TryParse(targetAccountIdString, out Guid targetAccountId))
            {
                foreach (var account in accounts)
                {
                    if (targetAccountId.ToString() == account.AccountId)
                    {
                        return account;
                    }
                }

                throw new Exception($"Targeted Account with Id {targetAccountId} not found.");
            }
            else
            {
                foreach (var account in accounts)
                {
                    if (bool.Parse(account.IsDefault))
                    {
                        return account;
                    }
                }
            }

            throw new Exception("The user does not have access to account");
        }
    }
}
