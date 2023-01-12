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
        private const string REDIRECT_URL = "https://developers.docusign.com/docs/esign-rest-api/";
        private const string REST_API_PREFIX = "/restapi";
        private const string DOCX_DOCUMENT_NAME = "World_Wide_Corp_salary.docx";

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
            string basePath = _testConfig.BasePath + REST_API_PREFIX;
            string tabsDocx = _testConfig.PathToSolution + DOCX_DOCUMENT_NAME;

            // Act
            var envelopeAndRedirectUrl = SetEnvelopeTabValue.CreateEnvelopeAndUpdateTabData(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                _testConfig.ImpersonatedUserId,
                _testConfig.AccessToken,
                basePath,
                _testConfig.AccountId,
                tabsDocx,
                REDIRECT_URL,
                REDIRECT_URL);

            string redirectUrl = envelopeAndRedirectUrl.Item2;
            string envelopeId = envelopeAndRedirectUrl.Item1;

            // Assert
            Assert.NotNull(redirectUrl);
            Assert.NotNull(envelopeId);
        }
    }
}
