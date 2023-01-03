using System;
using System.Collections.Generic;
using System.Text;
using DocuSign.CodeExamples.Common;
using ESignature.Examples;
using Xunit;

namespace launcher_csharp.Tests.eSignatureUnitTests
{
    public sealed class SetEnvelopeTabValueUnitTests
    {
        private const string RedirectUrl = "https://developers.docusign.com/docs/esign-rest-api/";

        private readonly ITestConfig _testConfig;

        public SetEnvelopeTabValueUnitTests() : this(TestConfig.Instance) { }

        private SetEnvelopeTabValueUnitTests(ITestConfig testConfig)
        {
            this._testConfig = testConfig;

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesAPIType.ESignature);
        }

        [Fact]
        public void SetEnvelopeTabValue_CorrectInputParameters_ReturnsEnvelopeIdAndRedirectURL()
        {
            // Arrange
            string basePath = _testConfig.BasePath + "/restapi";
            string tabsDocx = _testConfig.PathToSolution + "World_Wide_Corp_salary.docx";

            // Act
            var envelopeAndRedirectUrl = SetEnvelopeTabValue.CreateEnvelopeAndUpdateTabData(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                _testConfig.ImpersonatedUserId,
                _testConfig.AccessToken,
                basePath,
                _testConfig.AccountId,
                tabsDocx,
                RedirectUrl,
                RedirectUrl);

            string redirectUrl = envelopeAndRedirectUrl.Item2;
            string envelopeId = envelopeAndRedirectUrl.Item1;

            // Assert
            Assert.NotNull(redirectUrl);
            Assert.NotNull(envelopeId);
        }
    }
}
