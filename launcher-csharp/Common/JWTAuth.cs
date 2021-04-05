using System.Configuration;
using System.Linq;
using System.Text;
using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using static DocuSign.eSign.Client.Auth.OAuth.UserInfo;

namespace DocuSign.eSignature
{
    public static class JWTAuth
    {
        /// <summary>
        /// Uses Json Web Token (JWT) Authentication Method to obtain the necessary information needed to make API calls.
        /// </summary>
        /// <returns>A tuple containing the accessToken, accountId and baseUri</returns>
        public static (string, string, string) AuthenticateWithJWT()
        {
            var apiClient = new ApiClient();
            string ik = ConfigurationManager.AppSettings["IntegrationKey"];
            string userId = ConfigurationManager.AppSettings["userId"];
            string authServer = ConfigurationManager.AppSettings["AuthServer"];
            string rsaKey = ConfigurationManager.AppSettings["RSAKey"];
            OAuth.OAuthToken authToken = apiClient.RequestJWTUserToken(ik,
                            userId,
                            authServer,
                            Encoding.UTF8.GetBytes(rsaKey),
                            1);

            string accessToken = authToken.access_token;
            apiClient.SetOAuthBasePath(authServer);
            OAuth.UserInfo userInfo = apiClient.GetUserInfo(authToken.access_token);
            Account acct = null;

            var accounts = userInfo.Accounts;
            {
                acct = accounts.FirstOrDefault(a => a.IsDefault == "true");
            }
            string accountId = acct.AccountId;
            string baseUri = acct.BaseUri + "/restapi";
            return (accessToken, accountId, baseUri);
        }
    }
}