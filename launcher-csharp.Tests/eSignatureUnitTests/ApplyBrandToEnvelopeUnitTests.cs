using System;
using System.Collections.Generic;
using DocuSign.CodeExamples.Common;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using ESignature.Examples;
using FluentAssertions;
using Xunit;

namespace launcher_csharp.Tests.eSignatureUnitTests
{
    public sealed class ApplyBrandToEnvelopeUnitTests
    {
        private const string ESIGNARURE_PATH_PREFIX = "/restapi";

        private const string DOC_PDF = "World_Wide_Corp_lorem.pdf";

        private readonly ITestConfig _testConfig;

        public ApplyBrandToEnvelopeUnitTests() : this(TestConfig.Instance) { }

        private ApplyBrandToEnvelopeUnitTests(ITestConfig testConfig)
        {
            this._testConfig = testConfig;

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesAPIType.ESignature);
        }

        [Fact]
        public void CreateEnvelopeWithBranding_CorrectInputParameters_ReturnsEnvelopeSummary()
        {
            // Arrange
            GetBrands_CorrectInputParameters_ReturnsListOfBrands();

            string basePath = _testConfig.BasePath + ESIGNARURE_PATH_PREFIX;
            string docPdf = _testConfig.PathToSolution + DOC_PDF;
            string status = "sent";

            // Act
            EnvelopeSummary envelopeSummary = ApplyBrandToEnvelope.CreateEnvelopeWithBranding(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                _testConfig.BrandId,
                _testConfig.AccessToken,
                basePath,
                _testConfig.AccountId,
                status,
                docPdf);

            // Assert
            Assert.NotNull(envelopeSummary);
        }

        [Fact]
        public void CreateEnvelopeUsingBrand_CorrectInputParameters_ReturnsEnvelopeDefinition()
        {
            // Arrange
            GetBrands_CorrectInputParameters_ReturnsListOfBrands();
            string docPdf = _testConfig.PathToSolution + DOC_PDF;
            var status = "sent";
            var defaultId = "1";
            var fileExtension = "pdf";
            var fileName = "Lorem Ipsum";
            var anchorUnits = "pixels";
            var anchorYOffset = "10";
            var anchorXOffset = "20";
            var anchorStringOne = "**signature_1**";
            var anchorStringTwo = "/sn1/";
            var emailSubject = "Please sign this document set";

            string docPdfBytes = Convert.ToBase64String(System.IO.File.ReadAllBytes(docPdf));

            var expectedEnvelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = emailSubject
            };
            var document = new Document
            {
                DocumentBase64 = docPdfBytes,
                Name = fileName,
                FileExtension = fileExtension,
                DocumentId = defaultId,
            };

            expectedEnvelopeDefinition.Documents = new List<Document> { document };

            var signer = new Signer
            {
                Email = _testConfig.SignerEmail,
                Name = _testConfig.SignerName,
                RecipientId = defaultId,
                RoutingOrder = defaultId,
            };

            var signHere1 = new SignHere
            {
                AnchorString = anchorStringOne,
                AnchorUnits = anchorUnits,
                AnchorYOffset = anchorYOffset,
                AnchorXOffset = anchorXOffset,
            };

            var signHere2 = new SignHere
            {
                AnchorString = anchorStringTwo,
                AnchorUnits = anchorUnits,
                AnchorYOffset = anchorYOffset,
                AnchorXOffset = anchorXOffset,
            };

            var signerTabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere1, signHere2 },
            };
            signer.Tabs = signerTabs;

            var recipients = new Recipients
            {
                Signers = new List<Signer> { signer },
            };
            expectedEnvelopeDefinition.Recipients = recipients;
            expectedEnvelopeDefinition.Status = status;

            expectedEnvelopeDefinition.BrandId = _testConfig.BrandId;

            // Act
            EnvelopeDefinition envelopeDefinition = ApplyBrandToEnvelope.CreateEnvelope(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                _testConfig.BrandId,
                status,
                docPdf);

            // Assert
            Assert.NotNull(envelopeDefinition);
            envelopeDefinition.Should().BeEquivalentTo(expectedEnvelopeDefinition);
        }

        [Fact]
        public void GetBrands_CorrectInputParameters_ReturnsListOfBrands()
        {
            // Arrange
            var basePath = _testConfig.BasePath + ESIGNARURE_PATH_PREFIX;
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + _testConfig.AccessToken);

            var accountsApi = new AccountsApi(docuSignClient);

            // Act
            BrandsResponse brands = accountsApi.ListBrands(_testConfig.AccountId);

            // Assert
            Assert.NotNull(brands);
            Assert.NotEmpty(brands.Brands);

            Brand firstBrand = brands.Brands[0];
            Assert.NotNull(firstBrand);

            _testConfig.BrandId = firstBrand.BrandId;
        }
    }
}
