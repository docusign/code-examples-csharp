using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using System;
using Xunit;
using System.Runtime.InteropServices;
using DocuSign.CodeExamples.Authentication;
using DocuSign.CodeExamples.Common;

namespace launcher_csharp.Tests
{
    public sealed class JwtLoginMethodUnitTest
    {
        private const string REDIRECT_URL = "https://developers.docusign.com/platform/auth/consent";

        private const string RERUN_UNIT_TESTS = "Please, rerun the unit tests once consent has been provided.";

        private const string CONSENT_REQUIRED = "consent_required";

        [Theory]
        [InlineData(ExamplesAPIType.ESignature)]
        [InlineData(ExamplesAPIType.Monitor)]
        [InlineData(ExamplesAPIType.Click)]
        [InlineData(ExamplesAPIType.Rooms)]
        [InlineData(ExamplesAPIType.Admin)]
        public void RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesAPIType apiType, TestConfig _testConfig = null)
        {
            _testConfig = _testConfig ?? new TestConfig();

            // Arrange
            _testConfig.ApiClient = new DocuSignClient(_testConfig.Host);

            Console.WriteLine(_testConfig.ClientId);

            try
            {
                // Act
                OAuth.OAuthToken tokenInfo = JWTAuth.AuthenticateWithJWT(
                    apiType.ToString(),
                    _testConfig.ClientId,
                    _testConfig.ImpersonatedUserId,
                    _testConfig.OAuthBasePath,
                    _testConfig.PrivateKeyBytes
                    );

                OAuth.UserInfo userInfo = _testConfig.ApiClient.GetUserInfo(tokenInfo.access_token);

                // Assert
                Assert.NotNull(userInfo?.Accounts);

                var basePathAddition = "/restapi";
                var accountIsDefault = "true";

                foreach (OAuth.UserInfo.Account item in userInfo.Accounts)
                {
                    if (item.IsDefault == accountIsDefault)
                    {
                        _testConfig.AccountId = item.AccountId;
                        _testConfig.BasePath = item.BaseUri;
                        _testConfig.AccessToken = tokenInfo.access_token;
                        _testConfig.ApiClient.SetBasePath(item.BaseUri + basePathAddition);
                        break;
                    }
                }

                Assert.NotNull(_testConfig?.AccountId);
                Assert.NotNull(_testConfig?.BasePath);
                Assert.NotNull(_testConfig?.AccessToken);
            }
            catch (Exception e)
            {
                if (e.Message.ToLowerInvariant().Contains(CONSENT_REQUIRED))
                {
                    _testConfig?.OpenUrlUsingConsoleWindow(BuildConsentUrl(apiType, _testConfig));

                    Console.WriteLine("Consent url:");
                    Console.WriteLine(BuildConsentUrl(apiType, _testConfig));

                    throw new Xunit.Sdk.XunitException(RERUN_UNIT_TESTS);
                }
            }
        }

        private string BuildConsentUrl(ExamplesAPIType apiType, TestConfig _testConfig)
        {
            var scopes = "signature%20impersonation";
            if (apiType == ExamplesAPIType.Rooms)
            {
                scopes += "%20dtr.rooms.read%20dtr.rooms.write%20dtr.documents.read%20dtr.documents.write%20"
                          + "dtr.profile.read%20dtr.profile.write%20dtr.company.read%20dtr.company.write%20room_forms";
            }
            else if (apiType == ExamplesAPIType.Click)
            {
                scopes += "%20click.manage%20click.send";
            }
            else if (apiType == ExamplesAPIType.Admin)
            {
                scopes += "%20user_read%20user_write%20organization_read%20account_read%20group_read%20"
                            + "permission_read%20identity_provider_read%20domain_read%20user_data_redact";
            }

            string caret = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                caret = "^";
            }

            return "https://" + _testConfig.OAuthBasePath + "/oauth/auth?response_type=code" + caret + "&scope=" + scopes 
                   + caret + "&client_id=" + _testConfig.ClientId + caret + "&redirect_uri=" + REDIRECT_URL;
        }
    }
}
