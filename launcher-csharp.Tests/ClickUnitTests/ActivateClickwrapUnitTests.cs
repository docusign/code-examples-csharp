using System.Linq;
using DocuSign.Click.Examples;
using DocuSign.Click.Model;
using DocuSign.CodeExamples.Common;
using FluentAssertions;
using Xunit;

namespace launcher_csharp.Tests.ClickUnitTests
{
    [Collection("Click tests")]
    public sealed class ActivateClickwrapUnitTests
    {
        private const string ClickPathPrefix = "/clickapi";

        private readonly TestConfig _testConfig;

        private readonly CreateClickwrapUnitTests _createClickwrapUnitTests;

        public ActivateClickwrapUnitTests()
        {
            this._testConfig = new TestConfig();

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesApiType.Click, _testConfig);
            this._createClickwrapUnitTests = new CreateClickwrapUnitTests(_testConfig);
        }

        [Fact]
        public void BuildUpdateClickwrapVersionRequest_CorrectInputParameters_ReturnsClickwrapRequest()
        {
            // Arrange
            var statusActive = "active";
            var expectedClickwrapRequest = new ClickwrapRequest
            {
                Status = statusActive,
            };

            // Act
            ClickwrapRequest clickwrapRequest = ActivateClickwrap.BuildUpdateClickwrapVersionRequest();

            // Assert
            Assert.NotNull(clickwrapRequest);
            clickwrapRequest.Should().BeEquivalentTo(expectedClickwrapRequest);
        }


        [Fact]
        public void Update_CorrectInputParameters_ReturnsClickwrapVersionSummaryResponse()
        {
            // Arrange
            _createClickwrapUnitTests.Create_CorrectInputParameters_ReturnsClickwrapVersionSummaryResponse();

            string basePath = _testConfig.BasePath + ClickPathPrefix;
            var statusActive = "active";

            // Act
            ClickwrapVersionSummaryResponse clickwrapVersionSummaryResponse = ActivateClickwrap.Update(
                _testConfig.InactiveClickwrap.ClickwrapId,
                _testConfig.InactiveClickwrap.VersionNumber,
                basePath,
                _testConfig.AccessToken,
                _testConfig.AccountId
            );

            // Assert
            Assert.NotNull(clickwrapVersionSummaryResponse);
            clickwrapVersionSummaryResponse.Status.Should().BeEquivalentTo(statusActive);
        }
    }
}
