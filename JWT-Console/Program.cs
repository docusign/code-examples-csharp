using DocuSign.CodeExamples.Authentication;
using DocuSign.eSign.Client;
using static DocuSign.eSign.Client.Auth.OAuth;
using static DocuSign.eSign.Client.Auth.OAuth.UserInfo;
using ESignature.Examples;
using System;
using System.Diagnostics;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.IO;
using DocuSign.CodeExamples.Common;

namespace DocuSign.CodeExamples.JWT_Console
{
    class Program
    {
        static readonly string DevCenterPage = "https://developers.docusign.com/platform/auth/consent";

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            OAuthToken accessToken = null;
            try
            {
                accessToken = JwtAuth.AuthenticateWithJwt("ESignature", ConfigurationManager.AppSettings["ClientId"], ConfigurationManager.AppSettings["ImpersonatedUserId"],
                                                            ConfigurationManager.AppSettings["AuthServer"], DsHelper.ReadFileContent(ConfigurationManager.AppSettings["PrivateKeyFile"]));
            }
            catch (ApiException apiExp)
            {
                // Consent for impersonation must be obtained to use JWT Grant
                if (apiExp.Message.Contains("consent_required"))
                {
                    // Caret needed for escaping & in windows URL
                    string caret = "";
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        caret = "^";
                    }

                    // build a URL to provide consent for this Integration Key and this userId
                    string url = "https://" + ConfigurationManager.AppSettings["AuthServer"] + "/oauth/auth?response_type=code" + caret + "&scope=impersonation%20signature" + caret +
                        "&client_id=" + ConfigurationManager.AppSettings["ClientId"] + caret + "&redirect_uri=" + DevCenterPage;

                    string consentRequiredMessage = $"Consent is required - launching browser (URL is {url})";
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        consentRequiredMessage = consentRequiredMessage.Replace(caret, "");
                    }

                    Console.WriteLine(consentRequiredMessage);

                    // Start new browser window for login and consent to this app by DocuSign user
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = false });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", url);
                    }

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Unable to send envelope; Exiting. Please rerun the console app once consent was provided");
                    Console.ForegroundColor = ConsoleColor.White;
                    Environment.Exit(-1);
                }
            }

            var docuSignClient = new DocuSignClient();
            docuSignClient.SetOAuthBasePath(ConfigurationManager.AppSettings["AuthServer"]);
            UserInfo userInfo = docuSignClient.GetUserInfo(accessToken.access_token);
            Account acct = userInfo.Accounts.FirstOrDefault();

            Console.WriteLine("Welcome to the JWT Code example! ");
            Console.Write("Enter the signer's email address: ");
            string signerEmail = Console.ReadLine();
            Console.Write("Enter the signer's name: ");
            string signerName = Console.ReadLine();
            Console.Write("Enter the carbon copy's email address: ");
            string ccEmail = Console.ReadLine();
            Console.Write("Enter the carbon copy's name: ");
            string ccName = Console.ReadLine();
            string docDocx = Path.Combine(@"..", "..", "..", "..", "launcher-csharp", "World_Wide_Corp_salary.docx");
            string docPdf = Path.Combine(@"..", "..", "..", "..", "launcher-csharp", "World_Wide_Corp_lorem.pdf");
            Console.WriteLine("");
            string envelopeId = SigningViaEmail.SendEnvelopeViaEmail(signerEmail, signerName, ccEmail, ccName, accessToken.access_token, acct.BaseUri + "/restapi", acct.AccountId, docDocx, docPdf, "sent");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Successfully sent envelope with envelopeId {envelopeId}");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.White;
            Environment.Exit(0);
        }
    }
}
