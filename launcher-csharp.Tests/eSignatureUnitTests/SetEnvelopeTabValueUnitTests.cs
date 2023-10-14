using System;
using System.Collections.Generic;
using System.Text;
using DocuSign.CodeExamples.Common;
using ESignature.Examples;
using Xunit;

namespace launcher_csharp.Tests.eSignatureUnitTests
{
    [Collection("eSignature tests")]
    public sealed class SetEnvelopeTabValueUnitTests
    {
        private const string RedirectUrl = "https://developers.docusign.com/docs/esign-rest-api/";
        private const string RestApiPrefix = "/restapi";
        private const string DocxDocumentName = "World_Wide_Corp_salary.docx";

        private readonly TestConfig _testConfig;

        public SetEnvelopeTabValueUnitTests()
        {
            this._testConfig = new TestConfig();

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesApiType.ESignature, _testConfig);
        }

        [Fact]
        public void SetEnvelopeTabValue_CorrectInputParameters_ReturnsEnvelopeIdAndRedirectURL()
        {
            // Arrange
            string basePath = _testConfig.BasePath + RestApiPrefix;
            string tabsDocx = _testConfig.PathToSolution + DocxDocumentName;

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
