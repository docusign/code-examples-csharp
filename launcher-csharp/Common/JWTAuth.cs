using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
            var docuSignClient = new DocuSignClient();
            string ik = ConfigurationManager.AppSettings["IntegrationKey"];
            string userId = ConfigurationManager.AppSettings["userId"];
            string authServer = ConfigurationManager.AppSettings["AuthServer"];
            string rsaKeyFilePath = ConfigurationManager.AppSettings["KeyFilePath"];
            string selectedApiTypes = ConfigurationManager.AppSettings["SelectedApiTypes"] ?? "";

            List<string> scopes = new List<string>
            {
                "signature",
                "impersonation"
            };

            if (selectedApiTypes.Contains("Rooms"))
            {
                scopes.AddRange(new List<string>
                {
                    "dtr.rooms.read",
                    "dtr.rooms.write",
                    "dtr.documents.read",
                    "dtr.documents.write",
                    "dtr.profile.read",
                    "dtr.profile.write",
                    "dtr.company.read",
                    "dtr.company.write",
                    "room_forms"
                });
            }
            if (selectedApiTypes.Contains("Click"))
            {
                scopes.AddRange(new List<string>
                {
                    "click.manage",
                    "click.send"
                });
            }

            OAuth.OAuthToken authToken = docuSignClient.RequestJWTUserToken(ik,
                            userId,
                            authServer,
                            File.ReadAllBytes(rsaKeyFilePath),
                            1,
                            scopes);

            string accessToken = authToken.access_token;
            docuSignClient.SetOAuthBasePath(authServer);
            OAuth.UserInfo userInfo = docuSignClient.GetUserInfo(authToken.access_token);
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