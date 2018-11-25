using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using eg_03_csharp_auth_code_grant_core.Common;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace eg_03_csharp_auth_code_grant_core
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            DSConfiguration config = new DSConfiguration();
            
            Configuration.Bind("DocuSign", config);
            services.AddSingleton(config);
            services.AddScoped<IRequestItemsService, RequestItemsService>();

            services.AddMvc(options => {;
                options.Filters.Add<LocalsFilter>();
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMemoryCache();
            //services.AddCaching();// Adds a default in-memory implementation of IDistributedCache
            services.AddSession();
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            services.AddAuthentication(options => {
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

                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();
                        var user = JObject.Parse(await response.Content.ReadAsStringAsync());
                        
                        user.Add("access_token", context.AccessToken);
                        user.Add("refresh_token", context.RefreshToken);
                        user.Add("expires_in", DateTime.Now.Add(context.ExpiresIn.Value).ToString());
                        context.RunClaimActions(user);                        
                    },
                    OnRemoteFailure = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/Home/Error?message=" + context.Failure.Message);
                        return Task.FromResult(0);
                    }
                };
            });
        }

        private string ExtractDefaultAccountValue(JObject obj, string key)
        {
            if (!obj.ContainsKey("accounts"))
            {
                return null;
            }

            JToken account = (from a in obj["accounts"]
                              where a.Value<bool>("is_default") == true
                              select a).FirstOrDefault();


            return account.Value<string>(key);

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            
            app.UseAuthentication();
            
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
