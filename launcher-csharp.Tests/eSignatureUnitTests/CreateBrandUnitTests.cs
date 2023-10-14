using System;
using DocuSign.CodeExamples.Common;
using DocuSign.eSign.Model;
using ESignature.Examples;
using Xunit;

namespace launcher_csharp.Tests.eSignatureUnitTests
{
    [Collection("eSignature tests")]
    public sealed class CreateBrandUnitTests
    {
        private const string EsignarurePathPrefix = "/restapi";

        private readonly TestConfig _testConfig;

        public CreateBrandUnitTests()
        {
            this._testConfig = new TestConfig();

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesApiType.ESignature, _testConfig);
        }

        [Fact]
        public void CreateBrand_CorrectInputParameters_ReturnsBrandsResponse()
        {
            // Assert
            string brandName = Guid.NewGuid().ToString("n").Substring(0, 8);
            var defaultBrandLanguage = "en";
            string basePath = _testConfig.BasePath + EsignarurePathPrefix;

            //Act
            BrandsResponse permissionProfile = CreateBrand.Create(
                brandName,
                defaultBrandLanguage,
                _testConfig.AccessToken,
                basePath,
                _testConfig.AccountId);

            // Assert
            Assert.NotNull(permissionProfile);
            Assert.NotEmpty(permissionProfile.Brands);
        }
    }
}
