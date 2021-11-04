using DocuSign.CodeExamples.Authentication;
using DocuSign.eSign.Client;
using static DocuSign.eSign.Client.Auth.OAuth;
using static DocuSign.eSign.Client.Auth.OAuth.UserInfo;
using eSignature.Examples;
using System;
using System.Configuration;
using System.Linq;

namespace DocuSign.CodeExamples.JWT_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var accessToken = JWTAuth.AuthenticateWithJWT("ESignature", ConfigurationManager.AppSettings["ClientId"], ConfigurationManager.AppSettings["ImpersonatedUserId"], 
                                                        ConfigurationManager.AppSettings["AuthServer"], ConfigurationManager.AppSettings["PrivateKeyFile"]);
            var apiClient = new ApiClient();
            apiClient.SetOAuthBasePath(ConfigurationManager.AppSettings["AuthServer"]);
            UserInfo userInfo = apiClient.GetUserInfo(accessToken.access_token);
            Account acct = userInfo.Accounts.FirstOrDefault();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Welcome to the JWT Code example! ");
            Console.Write("Enter the signer's email address: ");
            string signerEmail = Console.ReadLine();
            Console.Write("Enter the signer's name: ");
            string signerName = Console.ReadLine();
            Console.Write("Enter the carbon copy's email address: ");
            string ccEmail = Console.ReadLine();
            Console.Write("Enter the carbon copy's name: ");
            string ccName = Console.ReadLine();
            string docDocx = @"..\..\..\..\launcher-csharp\World_Wide_Corp_salary.docx";
            string docPdf = @"..\..\..\..\launcher-csharp\World_Wide_Corp_lorem.pdf";
            Console.WriteLine("*****.....*****.....*****");
            string envelopeId = SigningViaEmail.SendEnvelopeViaEmail(signerEmail, signerName, ccEmail, ccName, accessToken.access_token, acct.BaseUri + "/restapi", acct.AccountId, docDocx, docPdf, "sent");
            Console.WriteLine("*****.....*****.....*****");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Successfully sent envelope with envelopeId {envelopeId}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
