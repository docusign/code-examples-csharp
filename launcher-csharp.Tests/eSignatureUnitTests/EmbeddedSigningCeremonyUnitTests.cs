using System;
using System.Collections.Generic;
using System.Text;
using DocuSign.CodeExamples.Common;
using DocuSign.eSign.Model;
using ESignature.Examples;
using FluentAssertions;
using Xunit;

namespace launcher_csharp.Tests.eSignatureUnitTests
{
    [Collection("eSignature tests")]
    public sealed class EmbeddedSigningCeremonyUnitTests
    {
        private const string RedirectUrl = "https://developers.docusign.com/docs/esign-rest-api/";

        private const string PdfDocumentName = "World_Wide_Corp_lorem.pdf";

        private const string RestApiPrefix = "/restapi";

        private readonly TestConfig _testConfig;

        public EmbeddedSigningCeremonyUnitTests()
        {
            this._testConfig = new TestConfig();

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesApiType.ESignature, _testConfig);
        }

        [Fact]
        public void EmbeddedSigningCeremony_CorrectInputParameters_ReturnsEnvelopeIdAndRedirectUrl()
        {
            // Arrange
            string docPdf = _testConfig.PathToSolution + PdfDocumentName;
            string basePath = _testConfig.BasePath + RestApiPrefix;

            // Act
            var result = EmbeddedSigningCeremony.SendEnvelopeForEmbeddedSigning(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                _testConfig.ImpersonatedUserId,
                _testConfig.AccessToken,
                basePath,
                _testConfig.AccountId,
                docPdf,
                RedirectUrl,
                RedirectUrl
            );

            // Assert
            Assert.NotNull(result.Item1);
            Assert.NotNull(result.Item2);
        }

        [Fact]
        public void MakeRecipientViewRequest_CorrectInputParameters_ReturnsViewRequest()
        {
            // Arrange
            var redirectSpecification = "?state=123";
            var authMethod = "none";
            var pingFrequency = "600";

            var expectedRecipientViewRequest = new RecipientViewRequest
            {
                ReturnUrl = RedirectUrl + redirectSpecification,
                AuthenticationMethod = authMethod,
                Email = _testConfig.SignerEmail,
                UserName = _testConfig.SignerName,
                ClientUserId = _testConfig.ImpersonatedUserId,
                PingFrequency = pingFrequency,
                PingUrl = RedirectUrl
            };

            // Act
            RecipientViewRequest recipientViewRequest = EmbeddedSigningCeremony.MakeRecipientViewRequest(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                RedirectUrl,
                _testConfig.ImpersonatedUserId,
                RedirectUrl
            );

            // Assert
            Assert.NotNull(recipientViewRequest);
            recipientViewRequest.Should().BeEquivalentTo(expectedRecipientViewRequest);
        }

        [Fact]
        public void MakeEnvelope_CorrectInputParameters_ReturnsEnvelopeDefinition()
        {
            // Arrange
            string docPdf = _testConfig.PathToSolution + PdfDocumentName;
            string anchorString = "/sn1/";
            var documentName = "Lorem Ipsum";
            var fileExtension = "pdf";
            var documentId = "3";
            var envelopeSubject = "Please sign this document";
            var status = "sent";
            var recipientId = "1";
            var anchorUnits = "pixels";
            var anchorXOffset = "10";
            var anchorYOffset = "20";

            var expectedEnvelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = envelopeSubject,
                Documents = new List<Document>
                {
                    new Document
                    {
                        DocumentBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(docPdf)),
                        Name = documentName,
                        FileExtension = fileExtension,
                        DocumentId = documentId
                    }
                },
                Status = status,
                Recipients = new Recipients
                {
                    Signers = new List<Signer>
                    {
                        new Signer
                        {
                            Email = _testConfig.SignerEmail,
                            Name = _testConfig.SignerName,
                            ClientUserId = _testConfig.ImpersonatedUserId,
                            RecipientId = recipientId,
                            Tabs = new Tabs
                            {
                                SignHereTabs = new List<SignHere>
                                {
                                    new SignHere
                                    {
                                        AnchorString = anchorString,
                                        AnchorUnits = anchorUnits,
                                        AnchorXOffset = anchorXOffset,
                                        AnchorYOffset = anchorYOffset,
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Act
            EnvelopeDefinition envelopeDefinition = EmbeddedSigningCeremony.MakeEnvelope(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                _testConfig.ImpersonatedUserId,
                docPdf
            );

            // Assert
            Assert.NotNull(envelopeDefinition);
            envelopeDefinition.Should().BeEquivalentTo(expectedEnvelopeDefinition);
        }
    }
}
