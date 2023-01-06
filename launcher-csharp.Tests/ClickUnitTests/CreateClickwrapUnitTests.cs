using System;
using System.Collections.Generic;
using DocuSign.Click.Examples;
using DocuSign.Click.Model;
using DocuSign.CodeExamples.Common;
using FluentAssertions;
using Xunit;

namespace launcher_csharp.Tests.ClickUnitTests
{
    public sealed class CreateClickwrapUnitTests
    {
        private const string CLICK_PATH_PREFIX = "/clickapi";

        private const string PDF_FILE = "Terms_of_service.pdf";

        private readonly ITestConfig _testConfig;

        public CreateClickwrapUnitTests() : this(TestConfig.Instance) { }

        private CreateClickwrapUnitTests(ITestConfig testConfig)
        {
            this._testConfig = testConfig;

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesAPIType.Click);
        }

        [Fact]
        public void BuildClickwrapRequest_CorrectInputParameters_ReturnsClickwrapRequest()
        {
            // Arrange
            var clickwrapName = "Clickwrap name";
            string pdfFile = _testConfig.PathToSolution + PDF_FILE;
            var consentButtonText = "I Agree";
            var format = "modal";
            var documentDisplay = "document";
            var termsOfService = "Terms of Service";
            var fileExtension = "pdf";

            var expectedClickwrapRequest = new ClickwrapRequest
            {
                DisplaySettings = new DisplaySettings()
                {
                    ConsentButtonText = consentButtonText,
                    DisplayName = clickwrapName,
                    Downloadable = true,
                    Format = format,
                    MustRead = true,
                    RequireAccept = true,
                    DocumentDisplay = documentDisplay,
                },
                Documents = new List<Document>()
                {
                    new Document()
                    {
                        DocumentBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(pdfFile)),
                        DocumentName = termsOfService,
                        FileExtension = fileExtension,
                        Order = 0,
                    },
                },
                Name = clickwrapName,
                RequireReacceptance = true,
            };

            // Act
            ClickwrapRequest clickwrapRequest = CreateClickwrap.BuildClickwrapRequest(clickwrapName, pdfFile);

            // Assert
            Assert.NotNull(clickwrapRequest);
            clickwrapRequest.Should().BeEquivalentTo(expectedClickwrapRequest);
        }

        [Fact]
        public void Create_CorrectInputParameters_ReturnsClickwrapVersionSummaryResponse()
        {
            // Arrange
            var clickwrapName = "Clickwrap name";
            string pdfFile = _testConfig.PathToSolution + PDF_FILE;
            string basePath = _testConfig.BasePath + CLICK_PATH_PREFIX;

            // Act
            ClickwrapVersionSummaryResponse clickwrapVersionSummaryResponse = CreateClickwrap.Create(
                clickwrapName,
                basePath,
                _testConfig.AccessToken,
                _testConfig.AccountId,
                pdfFile
                );

            // Assert
            Assert.NotNull(clickwrapVersionSummaryResponse);
            clickwrapVersionSummaryResponse.ClickwrapName.Should().Be(clickwrapName);
        }
    }
}
