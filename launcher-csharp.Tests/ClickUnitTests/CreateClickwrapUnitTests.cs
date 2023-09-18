using System;
using System.Collections.Generic;
using DocuSign.Click.Examples;
using DocuSign.Click.Model;
using DocuSign.CodeExamples.Common;
using FluentAssertions;
using Xunit;

namespace launcher_csharp.Tests.ClickUnitTests
{
    [Collection("Click tests")]
    public sealed class CreateClickwrapUnitTests
    {
        private const string ClickPathPrefix = "/clickapi";

        private const string PdfFile = "Terms_of_service.pdf";

        private readonly TestConfig _testConfig;

        public CreateClickwrapUnitTests(TestConfig testConfig = null)
        {
            this._testConfig = testConfig ?? new TestConfig();

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesApiType.Click, _testConfig);
        }

        [Fact]
        public void BuildClickwrapRequest_CorrectInputParameters_ReturnsClickwrapRequest()
        {
            // Arrange
            var clickwrapName = "Clickwrap name";
            string pdfFile = _testConfig.PathToSolution + PdfFile;
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
            var clickwrapName = Guid.NewGuid().ToString("n").Substring(0, 8);
            string pdfFile = _testConfig.PathToSolution + PdfFile;
            string basePath = _testConfig.BasePath + ClickPathPrefix;

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

            _testConfig.InactiveClickwrap = clickwrapVersionSummaryResponse;
        }
    }
}
