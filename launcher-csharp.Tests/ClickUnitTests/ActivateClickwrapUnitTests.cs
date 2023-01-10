using System.Linq;
using DocuSign.Click.Examples;
using DocuSign.Click.Model;
using DocuSign.CodeExamples.Common;
using FluentAssertions;
using Xunit;

namespace launcher_csharp.Tests.ClickUnitTests
{
    public sealed class ActivateClickwrapUnitTests
    {
        private const string CLICK_PATH_PREFIX = "/clickapi";

        private readonly ITestConfig _testConfig;

        private readonly CreateClickwrapUnitTests _createClickwrapUnitTests = new CreateClickwrapUnitTests();

        public ActivateClickwrapUnitTests() : this(TestConfig.Instance) { }

        private ActivateClickwrapUnitTests(ITestConfig testConfig)
        {
            this._testConfig = testConfig;

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesAPIType.Click);
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
        public void GetInactiveClickwraps_CorrectInputParameters_ReturnsClickwrapVersionsResponse()
        {
            // Arrange
            string basePath = _testConfig.BasePath + CLICK_PATH_PREFIX;
            var statusInactive = "inactive";

            // Act
            ClickwrapVersionsResponse clickwrapVersionSummaryResponse = ActivateClickwrap.GetInactiveClickwraps(
                basePath,
                _testConfig.AccessToken,
                _testConfig.AccountId
            );

            // Assert
            Assert.NotNull(clickwrapVersionSummaryResponse);
            clickwrapVersionSummaryResponse.Clickwraps
                .Count(x => x.Status == statusInactive)
                .Should()
                .Be(clickwrapVersionSummaryResponse.Clickwraps.Count());

            _testConfig.InactiveClickwrap = clickwrapVersionSummaryResponse.Clickwraps.FirstOrDefault();
        }

        [Fact]
        public void Update_CorrectInputParameters_ReturnsClickwrapVersionSummaryResponse()
        {
            // Arrange
            _createClickwrapUnitTests.Create_CorrectInputParameters_ReturnsClickwrapVersionSummaryResponse();
            GetInactiveClickwraps_CorrectInputParameters_ReturnsClickwrapVersionsResponse();
            string basePath = _testConfig.BasePath + CLICK_PATH_PREFIX;
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
