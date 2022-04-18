using System.Net.Http.Headers;
using System.Security.Claims;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Razor;
using DocuSign.CodeExamples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace DocuSign.QuickACG
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
            DSConfiguration config = new DSConfiguration();

            Configuration.Bind("DocuSign", config);
            config.QuickACG = "true";

            services.AddSingleton(config);
            services.AddScoped<IRequestItemsService, RequestItemsService>();
            services.AddMvc(options =>
            {
                options.Filters.Add<LocalsFilter>();
            });

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
                    },
                    OnRemoteFailure = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/Home/Error?message=" + context.Failure?.Message);
                        return Task.FromResult(0);
                    }
                };
            });
        }

        private string? ExtractDefaultAccountValue(JsonElement obj, string key)
        {
            if (!obj.TryGetProperty("accounts", out var accounts))
            {
                return null;
            }

            string? keyValue = null;

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

            return keyValue;
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
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            });
        }
    }
}
