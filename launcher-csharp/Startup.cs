// <copyright file="Startup.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

#nullable enable
namespace DocuSign.CodeExamples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using DocuSign.Rooms.Api;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.OAuth;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json.Linq;

    public class Startup
    {
        private readonly Dictionary<ExamplesApiType, List<string>> apiTypes = new Dictionary<ExamplesApiType, List<string>>();

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;

            this.apiTypes.Add(ExamplesApiType.ESignature, new List<string> { "signature", });

            this.apiTypes.Add(ExamplesApiType.Rooms, new List<string>
            {
                    "dtr.rooms.read",
                    "dtr.rooms.write",
                    "dtr.documents.read",
                    "dtr.documents.write",
                    "dtr.profile.read",
                    "dtr.profile.write",
                    "dtr.company.read",
                    "dtr.company.write",
                    "room_forms",
            });

            this.apiTypes.Add(ExamplesApiType.Click, new List<string>
            {
                    "click.manage",
                    "click.send",
            });

            this.apiTypes.Add(ExamplesApiType.Monitor, new List<string>
            {
                    "signature",
                    "impersonation",
            });

            this.apiTypes.Add(ExamplesApiType.Admin, new List<string>
            {
                    "signature",
                    "user_read",
                    "user_write",
                    "account_read",
                    "organization_read",
                    "group_read",
                    "permission_read",
                    "identity_provider_read",
                    "user_data_redact",
                    "asset_group_account_read",
                    "asset_group_account_clone_write",
                    "asset_group_account_clone_read",
            });

            this.apiTypes.Add(ExamplesApiType.WebForms, new List<string>
            {
                "signature", "webforms_read", "webforms_instance_write", "webforms_instance_read",
            });

            this.apiTypes.Add(ExamplesApiType.Maestro, new List<string> { "signature", "aow_manage" });
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RazorViewEngineOptions>(o =>
            {
                // {1} - controller
                // {0} - action
                o.ViewLocationFormats.Add("Views/Shared/{0}.cshtml");
                o.ViewLocationFormats.Add("Views/{1}/{0}.cshtml");
                o.ViewLocationFormats.Add($"/{{1}}/Views/{{0}}{RazorViewEngine.ViewExtension}");

                o.AreaViewLocationFormats.Clear();
                o.AreaViewLocationFormats.Add("/{2}/Views/{1}/{0}.cshtml");
                o.AreaViewLocationFormats.Add("/{2}/Views/Shared/{0}.cshtml");
                o.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });

            services.ConfigureNonBreakingSameSiteCookies();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });
            DsConfiguration config = new DsConfiguration();

            this.Configuration.Bind("DocuSign", config);
            this.Configuration["FirstLaunch"] = "true";

            services.AddSingleton(config);
            services.AddSingleton(new LauncherTexts(config, this.Configuration));
            services.AddScoped<IRequestItemsService, RequestItemsService>();
            services.AddScoped<IRoomsApi, RoomsApi>();
            services.AddScoped<IRolesApi, RolesApi>();
            services.AddScoped<IFormLibrariesApi, FormLibrariesApi>();
            services.AddScoped<IRoomTemplatesApi, RoomTemplatesApi>();
            services.AddScoped<IExternalFormFillSessionsApi, ExternalFormFillSessionsApi>();

            services.AddMvc(options =>
            {
                options.Filters.Add<LocalsFilter>();
            });

            services.AddRazorPages();
            services.AddMemoryCache();

            // services.AddCaching();// Adds a default in-memory implementation of IDistributedCache
            services.AddSession();
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication(options =>
            {
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "DocuSign";
            })
            .AddCookie()
            .AddOAuth("DocuSign", options =>
            {
                options.ClientId = this.Configuration["DocuSign:ClientId"];
                options.ClientSecret = this.Configuration["DocuSign:ClientSecret"];
                options.CallbackPath = new PathString("/ds/callback");
                options.AuthorizationEndpoint = this.Configuration["DocuSign:AuthorizationEndpoint"];
                options.TokenEndpoint = this.Configuration["DocuSign:TokenEndpoint"];
                options.UserInformationEndpoint = this.Configuration["DocuSign:UserInformationEndpoint"];

                string codeVerifier = GenerateCodeVerifier();
                string codeChallenge = GenerateCodeChallenge(codeVerifier);

                options.Events = new OAuthEvents
                {
                    OnRedirectToAuthorizationEndpoint = redirectContext =>
                    {
                        List<string> scopesForCurrentApi = this.apiTypes.GetValueOrDefault(Enum.Parse<ExamplesApiType>(this.Configuration["API"]));

                        var redirectUri = this.UpdateRedirectUriScopes(redirectContext.RedirectUri, scopesForCurrentApi);

                        var pkceQuery = $"&code_challenge={codeChallenge}&code_challenge_method=S256";
                        redirectContext.RedirectUri = redirectUri + pkceQuery;

                        redirectContext.HttpContext.Session.SetString("code_verifier", codeVerifier);

                        redirectContext.HttpContext.Response.Redirect(redirectContext.RedirectUri);
                        return Task.FromResult(0);
                    },
                    OnCreatingTicket = async context =>
                    {
                        string codeVerifier = context.HttpContext.Session.GetString("code_verifier");

                        var tokenRequestParams = new Dictionary<string, string>
                        {
                            { "grant_type", "authorization_code" },
                            { "code", context.ProtocolMessage.Code },
                            { "redirect_uri", context.Properties.RedirectUri },
                            { "client_id", options.ClientId },
                            { "code_verifier", codeVerifier },
                        };

                        var requestContent = new FormUrlEncodedContent(tokenRequestParams);
                        var requestMessage = new HttpRequestMessage(HttpMethod.Post, options.TokenEndpoint)
                        {
                            Content = requestContent
                        };
                        var response = await context.Backchannel.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                        context.RunClaimActions(payload.RootElement);
                    },
                    OnRemoteFailure = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/Home/Error?message=" + context.Failure?.Message);
                        return Task.FromResult(0);
                    },
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "Areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Rooms}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Click}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                            name: "default",
                            areaName: "Monitor",
                            pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                            name: "default",
                            pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");
            });
        }

        private string GenerateCodeVerifier()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[32];
                rng.GetBytes(bytes);
                return Base64UrlEncode(bytes);
            }
        }

        private string GenerateCodeChallenge(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                return Base64UrlEncode(hash);
            }
        }

        private string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        private string UpdateRedirectUriScopes(string uri, List<string> wantedScopes)
        {
            const string pattern = @"(?:&|\?)scope=([^&]+)";

            var encodedScopes = string.Join(" ", wantedScopes);
            return Regex.Replace(uri, pattern, $"&scope={HttpUtility.UrlPathEncode(encodedScopes)}");
        }

        private string? ExtractDefaultAccountValue(JsonElement obj, string key)
        {
            if (!obj.TryGetProperty("accounts", out var accounts))
            {
                return null;
            }

            string? keyValue = null;
            string targetAccountIdString = this.Configuration["DocuSign:TargetAccountId"];

            if (Guid.TryParse(targetAccountIdString, out Guid targetAccountId))
            {
                foreach (var account in accounts.EnumerateArray())
                {
                    account.TryGetProperty("account_id", out var accountIdJson);
                    accountIdJson.TryGetGuid(out Guid accountId);

                    if (targetAccountId == accountId)
                    {
                        if (account.TryGetProperty(key, out var value))
                        {
                            keyValue = value.GetString();
                        }
                    }
                }

                if (keyValue == null)
                {
                    string errorMessage = $"Targeted Account with Id {targetAccountId} not found.";
                    this.Configuration["ErrorMessage"] = errorMessage;

                    throw new Exception(errorMessage);
                }
            }
            else
            {
                foreach (var account in accounts.EnumerateArray())
                {
                    if (account.TryGetProperty("is_default", out var defaultAccount) && defaultAccount.GetBoolean())
                    {
                        if (account.TryGetProperty(key, out var value))
                        {
                            keyValue = value.GetString();
                        }
                    }
                }
            }

            return keyValue;
        }
    }
}
