using DocuSign.CodeExamples;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Models;
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
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace DocuSign.QuickACG
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RazorViewEngineOptions>(o =>
            {
                // {1} - controller
                // {0} - action
                o.AreaViewLocationFormats.Clear();
                o.ViewLocationFormats.Clear();
                o.AreaViewLocationFormats.Add("Views/Eg001EmbeddedSigning/quickEmbeddedSigning" + RazorViewEngine.ViewExtension);
                o.ViewLocationFormats.Add("Views/Eg001EmbeddedSigning/quickEmbeddedSigning" + RazorViewEngine.ViewExtension);
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });
            DsConfiguration config = new DsConfiguration();

            Configuration.Bind("DocuSign", config);
            config.QuickAcg = "true";

            services.AddSingleton(config);
            services.AddSingleton(new LauncherTexts(config, Configuration));
            services.AddScoped<IRequestItemsService, RequestItemsService>();
            services.AddMvc();

            services.AddRazorPages();
            services.AddMemoryCache();
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
                options.ClientId = Configuration["DocuSign:ClientId"];
                options.ClientSecret = Configuration["DocuSign:ClientSecret"];
                options.CallbackPath = new PathString("/ds/callback");

                options.AuthorizationEndpoint = Configuration["DocuSign:AuthorizationEndpoint"];
                options.TokenEndpoint = Configuration["DocuSign:TokenEndpoint"];
                options.UserInformationEndpoint = Configuration["DocuSign:UserInformationEndpoint"];
                options.Scope.Add("signature");

                options.SaveTokens = true;
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey("accounts", "accounts");

                options.ClaimActions.MapCustomJson("account_id", obj => ExtractDefaultAccountValue(obj, "account_id"));
                options.ClaimActions.MapCustomJson("account_name", obj => ExtractDefaultAccountValue(obj, "account_name"));
                options.ClaimActions.MapCustomJson("base_uri", obj => ExtractDefaultAccountValue(obj, "base_uri"));
                options.ClaimActions.MapJsonKey("access_token", "access_token");
                options.ClaimActions.MapJsonKey("refresh_token", "refresh_token");
                options.ClaimActions.MapJsonKey("expires_in", "expires_in");
                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        HttpResponseMessage response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();
                        var userJObject = JObject.Parse(await response.Content.ReadAsStringAsync());

                        userJObject.Add("access_token", context.AccessToken);
                        userJObject.Add("refresh_token", context.RefreshToken);
                        userJObject.Add("expires_in", DateTime.Now.Add(context.ExpiresIn.Value).ToString());

                        using (JsonDocument payload = JsonDocument.Parse(userJObject.ToString()))
                        {
                            context.RunClaimActions(payload.RootElement);
                        }
                    }
                };
            });
        }

        #nullable enable
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
                    throw new Exception($"Targeted Account with Id {targetAccountId} not found.");
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
        #nullable disable

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
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
                    name: "default",
                    pattern: "{controller=Eg}/{action=Index}");
            });
        }
    }
}
