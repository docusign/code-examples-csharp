#region NetFramework
// TODO: 
// 1. add local filters to your FilterConfig, for example: filters.Add(new LocalsFilter(DSConfiguration.Instance, RequestItemsService.Instance));
// 2. Use attribute for register startup for OWIN: [assembly: OwinStartupAttribute(typeof(DocuSign.eSignature.DSOwinStartup))]
// Use DSChallengeResult to authenticate via DS.

namespace DocuSign.eSignature
{
	using Microsoft.AspNet.Identity;
	using Microsoft.Owin;
	using Microsoft.Owin.Infrastructure;
	using Microsoft.Owin.Security;
	using Microsoft.Owin.Security.Cookies;
	using Microsoft.Owin.Security.DataHandler;
	using Microsoft.Owin.Security.DataProtection;
	using Microsoft.Owin.Security.Infrastructure;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Owin;
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Net;
	using System.Runtime.Caching;
	using System.Security.Claims;
	using System.Text;
	using System.Threading.Tasks;
	using System.Web;
	using System.Web.Mvc;

	public class DSOwinStartup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}

		public void ConfigureAuth(IAppBuilder app)
		{

			app.UseCookieAuthentication(new CookieAuthenticationOptions());

			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

			app.Use(typeof(DocuSignAuthenticationMiddleware), app, new DocuSignAuthenticationOptions
			{
				ClientId = ConfigurationManager.AppSettings["IntegrationKey"],
				ClientSecret = ConfigurationManager.AppSettings["SecretKey"],
				AuthorizationEndpoint = ConfigurationManager.AppSettings["AuthorizationEndpoint"],
				TokenEndpoint = ConfigurationManager.AppSettings["TokenEndpoint"],
				UserInformationEndpoint = ConfigurationManager.AppSettings["UserInformationEndpoint"],
				AppUrl = ConfigurationManager.AppSettings["AppUrl"],
				SignerEmail = ConfigurationManager.AppSettings["SignerEmail"],
				SignerName = ConfigurationManager.AppSettings["SignerName"],
				GatewayAccountId = ConfigurationManager.AppSettings["GatewayAccountId"],
				GatewayName = ConfigurationManager.AppSettings["GatewayName"],
				GatewayDisplayName = ConfigurationManager.AppSettings["GatewayDisplayName"],
				CallbackPath = new PathString("/ds/Callback")
			});
		}
	}

	public static class Constants
	{
		internal const string DefaultAuthenticationType = "DocuSign";
	}

	public class DocuSignAuthenticationHandler : AuthenticationHandler<DocuSignAuthenticationOptions>
	{
		protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
		{
			var authResult = Authenticate();
			var userInfo = GetUserInfo(ExtractDefaultAccountValue(authResult, "access_token"));


			// ASP.Net Identity requires the NameIdentitifer field to be set or it won't  
			// accept the external login (AuthenticationManagerExtensions.GetExternalLoginInfo)

			var claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.NameIdentifier, ExtractDefaultAccountValue(userInfo, "sub")));
			claims.Add(new Claim(ClaimTypes.Name, ExtractDefaultAccountValue(userInfo, "name")));
			claims.Add(new Claim("accounts", ExtractDefaultAccountValue(userInfo, "accounts")));
			claims.Add(new Claim("access_token", ExtractDefaultAccountValue(authResult, "access_token")));
			claims.Add(new Claim("refresh_token", ExtractDefaultAccountValue(authResult, "refresh_token")));

			var requestItemsService = RequestItemsService.Instance as RequestItemsService;

			var user = new User
			{
				AccessToken = ExtractDefaultAccountValue(authResult, "access_token"),
				RefreshToken = ExtractDefaultAccountValue(authResult, "refresh_token"),
				Name = ExtractDefaultAccountValue(userInfo, "name")
			};

			var session = new Session
			{
				AccountId = ExtractDefaultAccountValue(userInfo, "account_id"),
				AccountName = ExtractDefaultAccountValue(userInfo, "account_name"),
				BasePath = ExtractDefaultAccountValue(userInfo, "base_uri")
			};


			if (double.TryParse(ExtractDefaultAccountValue(authResult, "expires_in"), out var expiresIn))
			{
				claims.Add(new Claim("expires_in", DateTime.Now.Add(TimeSpan.FromSeconds(expiresIn)).ToString()));
				user.ExpireIn = DateTime.Now.Add(TimeSpan.FromSeconds(expiresIn));
			}

			requestItemsService.UpdateValues(user, session, HttpContext.Current.User.Identity.Name);

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType);

			// Query - code == state
			var properties = Options.StateDataFormat.Unprotect(Request.Query["state"]);

			return Task.FromResult(new AuthenticationTicket(identity, properties));
		}

		protected override Task ApplyResponseChallengeAsync()
		{
			if (Response.StatusCode == 401)
			{
				var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

				// Only react to 401 if there is an authentication challenge for the authentication 
				// type of this handler.
				if (challenge != null)
				{
					var state = challenge.Properties;

					if (string.IsNullOrEmpty(state.RedirectUri))
					{
						state.RedirectUri = Request.Uri.ToString();
					}

					var stateString = Options.StateDataFormat.Protect(state);

					var selectedApiTypes = ConfigurationManager.AppSettings["SelectedApiTypes"] ?? "";

					var scopes = "signature impersonation ";

					if (selectedApiTypes.Contains("Rooms"))
					{
						scopes += "dtr.rooms.read dtr.rooms.write dtr.documents.read dtr.documents.write" +
						  " dtr.profile.read dtr.profile.write dtr.company.read dtr.company.write room_forms ";
					}
					if (selectedApiTypes.Contains("Click"))
					{
						scopes += "click.manage click.send";
					}

					Response.Redirect(WebUtilities.AddQueryString(Options.AuthorizationEndpoint,
						new Dictionary<string, string>{
							{ "client_id", Options.ClientId },
							{ "scope", scopes },
							{ "response_type", "code" },
							{ "redirect_uri", Options.AppUrl + Options.CallbackPath.ToString() },
							{ "state", stateString }
						}));
				}
			}

			return Task.FromResult<object>(null);
		}

		public override async Task<bool> InvokeAsync()
		{
			// Handle response from oauth server
			if (Options.CallbackPath.HasValue && Options.CallbackPath == Request.Path)
			{
				var ticket = await AuthenticateAsync();

				if (ticket != null)
				{
					Context.Authentication.SignIn(ticket.Properties, ticket.Identity);

					Response.Redirect(ticket.Properties.RedirectUri);

					// Prevent further processing by the owin pipeline.
					return true;
				}
			}
			// Let the rest of the pipeline run.
			return false;
		}

		private JObject Authenticate()
		{
			JObject authResult;
			using (WebClient wc = new WebClient())
			{
				wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
				wc.Headers[HttpRequestHeader.Authorization] = "Basic " +
					Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Options.ClientId}:{Options.ClientSecret}"));

				var request = $"grant_type=authorization_code&code={Request.Query["code"]}";

				var result = wc.UploadString(Options.TokenEndpoint, "POST", request);
				authResult = JObject.Parse(result);
			}

			return authResult;
		}

		private JObject GetUserInfo(string jwt)
		{
			JObject authResult;
			using (WebClient wc = new WebClient())
			{
				wc.Headers[HttpRequestHeader.Authorization] = $"Bearer {jwt}";

				var result = wc.DownloadString(Options.UserInformationEndpoint);
				authResult = JObject.Parse(result);
			}

			return authResult;
		}

		private string ExtractDefaultAccountValue(JObject obj, string key)
		{
			if (obj.ContainsKey(key))
			{
				return obj[key].ToString();
			}
			else if (obj.ContainsKey("accounts"))
			{

				JToken account = (from a in obj["accounts"]
								  where a.Value<bool>("is_default") == true
								  select a).FirstOrDefault();

				return account.Value<string>(key);
			}
			return null;
		}
	}

	public class DocuSignAuthenticationMiddleware : AuthenticationMiddleware<DocuSignAuthenticationOptions>
	{
		public DocuSignAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, DocuSignAuthenticationOptions options)
			: base(next, options)
		{
			//  if(string.IsNullOrEmpty(Options.SignInAsAuthenticationType))
			//  {
			//      options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();
			//  }
			if (options.StateDataFormat == null)
			{
				var dataProtector = app.CreateDataProtector(typeof(DocuSignAuthenticationMiddleware).FullName,
					options.AuthenticationType);

				options.StateDataFormat = new PropertiesDataFormat(dataProtector);
			}
		}

		// Called for each request, to create a handler for each request.
		protected override AuthenticationHandler<DocuSignAuthenticationOptions> CreateHandler()
		{
			return new DocuSignAuthenticationHandler();
		}
	}

	public class DSChallengeResult : HttpUnauthorizedResult
	{
		public DSChallengeResult(string provider = "DocuSign", string redirectUri = "/")
			: this(provider, redirectUri, null)
		{
		}

		public DSChallengeResult(string provider, string redirectUri, string userId)
		{
			LoginProvider = provider;
			RedirectUri = redirectUri;
			UserId = userId;
		}

		public string LoginProvider { get; set; }
		public string RedirectUri { get; set; }
		public string UserId { get; set; }

		public override void ExecuteResult(ControllerContext context)
		{
			var properties = new AuthenticationProperties { RedirectUri = RedirectUri };

			context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
		}
	}

	public class DocuSignAuthenticationOptions : AuthenticationOptions
	{
		public DocuSignAuthenticationOptions()
			: base(Constants.DefaultAuthenticationType)
		{
			Description.Caption = Constants.DefaultAuthenticationType;
			AuthenticationMode = AuthenticationMode.Passive;
		}

		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string AuthorizationEndpoint { get; set; }
		public string TokenEndpoint { get; set; }
		public string UserInformationEndpoint { get; set; }
		public string AppUrl { get; set; }
		public string SignerEmail { get; set; }
		public string SignerName { get; set; }
		public string GatewayAccountId { get; set; }
		public string GatewayName { get; set; }
		public string GatewayDisplayName { get; set; }
		public PathString CallbackPath { get; set; }

		public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
	}

	public class RequestItemsService : IRequestItemsService
	{
		private readonly MemoryCache _cache;

		private string _id;

		private string _accessToken;

		public RequestItemsService(MemoryCache cache)
		{
			_cache = MemoryCache.Default;
		}

		public static RequestItemsService Instance { get; private set; } = new RequestItemsService(MemoryCache.Default);

		public void UpdateValues(User user, Session session, string id)
		{
			_id = id;
			_accessToken = user.AccessToken;
			User = user;
			Session = session;
		}

		private string GetKey(string key)
		{
			return string.Format("{0}_{1}", _id, key);
		}

		public Session Session
		{
			get => _cache.Get(GetKey("Session")) as Session;

			set => _cache.Add(new CacheItem(GetKey("Session"), value), new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + TimeSpan.FromSeconds(100) });
		}

		public User User
		{
			get => _cache.Get(GetKey("User")) as User;
			set => _cache.Add(new CacheItem(GetKey("User"), value), new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + TimeSpan.FromSeconds(100) });
		}
	}

	public static class SessionExtensions
	{
		public static void SetObjectAsJson(this HttpSessionStateBase session, string key, object value)
		{
			session[key] = JsonConvert.SerializeObject(value);
		}

		public static T GetObjectFromJson<T>(this HttpSessionStateBase session, string key)
		{
			var value = session[key]?.ToString();

			return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
		}
	}

	public class LocalsFilter : IActionFilter
	{
		DSConfiguration Config { get; }

		private readonly IRequestItemsService _requestItemsService;

		public LocalsFilter(DSConfiguration config, IRequestItemsService requestItemsService)
		{
			Config = config;
			_requestItemsService = requestItemsService;
		}

		public void OnActionExecuted(ActionExecutedContext filterContext)
		{
		}

		public void OnActionExecuting(ActionExecutingContext filterContext)
		{

			var controller = filterContext.Controller as Controller;

			if (controller == null)
			{
				return;
			}
			var viewBag = controller.ViewBag;
			var httpContext = filterContext.HttpContext;

			var locals = httpContext.Session.GetObjectFromJson<Locals>("locals") ?? new Locals();
			locals.DsConfig = Config;
			locals.Session = httpContext.Session.GetObjectFromJson<Session>("session") ?? null;
			locals.Messages = "";
			locals.Json = null;
			locals.User = null;
			viewBag.Locals = locals;
			viewBag.showDoc = Config.documentation != null;


			var identity = httpContext.User.Identity as ClaimsIdentity;
			if (identity != null && !identity.IsAuthenticated)
			{
				locals.Session = new Session();
				return;
			}


			locals.User = httpContext.Session.GetObjectFromJson<User>("user");

			if (locals.User == null)
			{
				locals.User = _requestItemsService.User;
			}
			if (locals.Session == null)
			{
				locals.Session = _requestItemsService.Session;
			}
		}
	}
}
#endregion

#region NetCore
// TODO: add following lines to your Startup.cs:
// 1. ConfigureMVCForDS to 'ConfigureServices' method, for example: services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).ConfigureMVCForDS();
// 2. ConfigureDS to 'ConfigureServices' method, for example: services.ConfigureDS(Configuration);
// 3. app.ConfigureDS(); to 'Configure' method, for example: app.ConfigureDS();
namespace DocuSign.eSignature
{

	using Microsoft.AspNetCore.Authentication;
	using Microsoft.AspNetCore.Authentication.Cookies;
	using Microsoft.AspNetCore.Authentication.OAuth;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Filters;
	using Microsoft.Extensions.Caching.Memory;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
    using System.Collections.Generic;
    using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Security.Claims;
	using System.Threading.Tasks;

	public static class DocuSignWebAppExtensions
	{
		public static void ConfigureDS(this IApplicationBuilder applicationBuilder)
		{
			applicationBuilder.UseAuthentication();
			applicationBuilder.UseSession();
		}

		public static IMvcBuilder ConfigureMVCForDS(this IMvcBuilder mvcBuilder) =>
			mvcBuilder.AddMvcOptions(options => options.Filters.Add<LocalsFilter>());

		public static void ConfigureDS(this IServiceCollection services, IConfiguration Configuration)
		{
			DSConfiguration config = new DSConfiguration();

			Configuration.Bind("DocuSign", config);
			services.AddSingleton(config);
			services.AddScoped<IRequestItemsService, RequestItemsService>();

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
				options.ClientId = Configuration["DocuSign:IntegrationKey"];
				options.ClientSecret = Configuration["DocuSign:SecretKey"];
				options.CallbackPath = new PathString("/ds/callback");

				options.AuthorizationEndpoint = Configuration["DocuSign:AuthorizationEndpoint"];
				options.TokenEndpoint = Configuration["DocuSign:TokenEndpoint"];
				options.UserInformationEndpoint = Configuration["DocuSign:UserInformationEndpoint"];

				options.Scope.Add("signature");
				options.Scope.Add("impersonation");

				var selectedApiTypes = Configuration.GetSection("DocuSign:examplesApi")
					.Get<Dictionary<string, bool>>();

				if (selectedApiTypes != null)
                {
					if (selectedApiTypes.ContainsKey("isRoomsApi") && selectedApiTypes["isRoomsApi"])
					{
						options.Scope.Add("dtr.rooms.read");
						options.Scope.Add("dtr.rooms.write");
						options.Scope.Add("dtr.documents.read");
						options.Scope.Add("dtr.documents.write");
						options.Scope.Add("dtr.profile.read");
						options.Scope.Add("dtr.profile.write");
						options.Scope.Add("dtr.company.read");
						options.Scope.Add("dtr.company.write");
						options.Scope.Add("room_forms");
					}
					if (selectedApiTypes.ContainsKey("isClickApi") && selectedApiTypes["isClickApi"])
					{
						options.Scope.Add("click.manage");
						options.Scope.Add("click.send");
					}
				}

				options.SaveTokens = true;
				options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
				options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
				options.ClaimActions.MapJsonKey("accounts", "accounts");

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
						var responce = JObject.Parse(await response.Content.ReadAsStringAsync());
						var ris = context.HttpContext.RequestServices.GetService(typeof(IRequestItemsService)) as RequestItemsService;
						var session = new Session
						{
							AccountId = responce["accounts"][0]["account_id"].ToString(),
							AccountName = responce["accounts"][0]["account_name"].ToString(),
							BasePath = responce["accounts"][0]["base_uri"].ToString()
						};
						var user = new User
						{
							AccessToken = context.AccessToken,
							RefreshToken = context.RefreshToken,
							ExpireIn = DateTime.Now.Add(context.ExpiresIn.Value),
							Name = responce["name"].ToString()
						};
						ris.UpdateValues(user, session, context.HttpContext.User.Identity.Name);
						context.RunClaimActions();
					},
					OnRemoteFailure = context =>
					{
						context.HandleResponse();
						context.Response.Redirect("/Error?message=" + context.Failure.Message);
						return Task.FromResult(0);
					}
				};
			});
		}
	}

	public class LocalsFilter : IActionFilter
	{
		DSConfiguration Config { get; }

		private readonly IRequestItemsService _requestItemsService;
		private IMemoryCache _cache;

		public LocalsFilter(DSConfiguration config, IRequestItemsService requestItemsService, IMemoryCache cache)
		{
			Config = config;
			_cache = cache;
			_requestItemsService = requestItemsService;
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{

		}

		public void OnActionExecuting(ActionExecutingContext context)
		{

			Controller controller = context.Controller as Controller;

			if (controller == null)
			{
				return;
			}
			var viewBag = controller.ViewBag;
			var httpContext = context.HttpContext;

			var locals = httpContext.Session.GetObjectFromJson<Locals>("locals") ?? new Locals();
			locals.DsConfig = Config;
			locals.Session = httpContext.Session.GetObjectFromJson<Session>("session") ?? null;
			locals.Messages = "";
			locals.Json = null;
			locals.User = null;
			viewBag.Locals = locals;
			viewBag.showDoc = Config.documentation != null;


			var identity = httpContext.User.Identity as ClaimsIdentity;
			if (identity != null && !identity.IsAuthenticated)
			{
				locals.Session = new Session();
				return;
			}


			locals.User = httpContext.Session.GetObjectFromJson<User>("user");

			if (locals.User == null)
			{
				locals.User = _requestItemsService.User;
			}
			if (locals.Session == null)
			{
				locals.Session = _requestItemsService.Session;
			}
		}
	}

	public class RequestItemsService : IRequestItemsService
	{
		private readonly IMemoryCache _cache;
		private string _id;
		private string _accessToken;

		public RequestItemsService(IMemoryCache cache)
		{
			_cache = cache;
		}

		public void UpdateValues(User user, Session session, string id)
		{
			_id = id;
			_accessToken = user.AccessToken;
			User = user;
			Session = session;
		}

		private string GetKey(string key)
		{
			return string.Format("{0}_{1}", _id, key);
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
	}

	public static class SessionExtensions
	{
		public static void SetObjectAsJson(this ISession session, string key, object value)
		{
			session.SetString(key, JsonConvert.SerializeObject(value));
		}

		public static T GetObjectFromJson<T>(this ISession session, string key)
		{
			var value = session.GetString(key);

			return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
		}
	}
}
#endregion

namespace DocuSign.eSignature
{
	using System;

	public class Locals
	{
		public DSConfiguration DsConfig { get; set; }
		public User User { get; set; }
		public Session Session { get; set; }
		public String Messages { get; set; }
		public object Json { get; internal set; }
	}

	public class DSConfiguration
	{
		public string AppUrl { get; set; }

		public string SignerEmail { get; set; }

		public string SignerName { get; set; }

		public string GatewayAccountId { get; set; }

		public string GatewayName { get; set; }

		public string GatewayDisplayName { get; set; }

		public bool production = false;
		public bool debug = true; // Send debugging statements to console
		public string sessionSecret = "12345"; // Secret for encrypting session cookie content
		public bool allowSilentAuthentication = true; // a user can be silently authenticated if they have an
													  // active login session on another tab of the same browser
													  // Set if you want a specific DocuSign AccountId, If null, the user's default account will be used.
		public string targetAccountId = null;
		public string demoDocPath = "demo_documents";
		public string docDocx = "World_Wide_Corp_Battle_Plan_Trafalgar.docx";
		public string tabsDocx = "World_Wide_Corp_salary.docx";
		public string docPdf = "World_Wide_Corp_lorem.pdf";
		public string githubExampleUrl = "https://github.com/docusign/eg-03-csharp-auth-code-grant-core/tree/master/eg-03-csharp-auth-code-grant-core/Controllers/";
		public string documentation = null;

		public static DSConfiguration Instance { get; private set; } = new DSConfiguration();
	}

	public class Session
	{
		public string AccountId { get; set; }
		public string AccountName { get; set; }
		public string BasePath { get; set; }
	}

	public class User
	{
		public string Name { get; set; }
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public DateTime? ExpireIn { get; set; }
	}

	public interface IRequestItemsService
	{
		Session Session { get; set; }

		User User { get; set; }
	}
}