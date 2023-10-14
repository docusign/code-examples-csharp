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
        private const string RedirectUrl = "https://developers.docusign.com/platform/auth/consent";

        private const string RerunUnitTests = "Please, rerun the unit tests once consent has been provided.";

        private const string ConsentRequired = "consent_required";

        [Theory]
        [InlineData(ExamplesApiType.ESignature)]
        [InlineData(ExamplesApiType.Monitor)]
        [InlineData(ExamplesApiType.Click)]
        [InlineData(ExamplesApiType.Rooms)]
        [InlineData(ExamplesApiType.Admin)]
        public void RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesApiType apiType, TestConfig testConfig = null)
        {
            testConfig = testConfig ?? new TestConfig();

            // Arrange
            testConfig.ApiClient = new DocuSignClient(testConfig.Host);

            try
            {
                // Act
                OAuth.OAuthToken tokenInfo = JwtAuth.AuthenticateWithJwt(
                    apiType.ToString(),
                    testConfig.ClientId,
                    testConfig.ImpersonatedUserId,
                    testConfig.OAuthBasePath,
                    testConfig.PrivateKeyBytes
                    );

                OAuth.UserInfo userInfo = testConfig.ApiClient.GetUserInfo(tokenInfo.access_token);

                // Assert
                Assert.NotNull(userInfo?.Accounts);

                var basePathAddition = "/restapi";
                var accountIsDefault = "true";

                foreach (OAuth.UserInfo.Account item in userInfo.Accounts)
                {
                    if (item.IsDefault == accountIsDefault)
                    {
                        testConfig.AccountId = item.AccountId;
                        testConfig.BasePath = item.BaseUri;
                        testConfig.AccessToken = tokenInfo.access_token;
                        testConfig.ApiClient.SetBasePath(item.BaseUri + basePathAddition);
                        break;
                    }
                }

                Assert.NotNull(testConfig?.AccountId);
                Assert.NotNull(testConfig?.BasePath);
                Assert.NotNull(testConfig?.AccessToken);
            }
            catch (Exception e)
            {
                if (e.Message.ToLowerInvariant().Contains(ConsentRequired))
                {
                    testConfig?.OpenUrlUsingConsoleWindow(BuildConsentUrl(apiType, testConfig));

                    throw new Xunit.Sdk.XunitException(RerunUnitTests);
                }
            }
        }

        private string BuildConsentUrl(ExamplesApiType apiType, TestConfig testConfig)
        {
            var scopes = "signature%20impersonation";
            if (apiType == ExamplesApiType.Rooms)
            {
                scopes += "%20dtr.rooms.read%20dtr.rooms.write%20dtr.documents.read%20dtr.documents.write%20"
                          + "dtr.profile.read%20dtr.profile.write%20dtr.company.read%20dtr.company.write%20room_forms";
            }
            else if (apiType == ExamplesApiType.Click)
            {
                scopes += "%20click.manage%20click.send";
            }
            else if (apiType == ExamplesApiType.Admin)
            {
                scopes += "%20user_read%20user_write%20organization_read%20account_read%20group_read%20"
                            + "permission_read%20identity_provider_read%20domain_read%20user_data_redact%20"
                            + "asset_group_account_read%20asset_group_account_clone_write%20asset_group_account_clone_read";
            }

            string caret = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                caret = "^";
            }

            return "https://" + testConfig.OAuthBasePath + "/oauth/auth?response_type=code" + caret + "&scope=" + scopes 
                   + caret + "&client_id=" + testConfig.ClientId + caret + "&redirect_uri=" + RedirectUrl;
        }
    }
}
