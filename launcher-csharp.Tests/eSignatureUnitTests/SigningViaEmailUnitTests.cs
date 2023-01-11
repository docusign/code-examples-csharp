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
    public sealed class SigningViaEmailUnitTests
    {
        private const string CC_MAIL = "cc@gmail.com";

        private const string CC_NAME = "CC";

        private const string PDF_DOCUMENT_NAME = "World_Wide_Corp_lorem.pdf";

        private const string DOCX_DOCUMENT_NAME = "World_Wide_Corp_Battle_Plan_Trafalgar.docx";

        private const string REST_API_PREFIX = "/restapi";

        private readonly ITestConfig _testConfig;

        public SigningViaEmailUnitTests() : this(TestConfig.Instance) { }

        private SigningViaEmailUnitTests(ITestConfig testConfig)
        {
            this._testConfig = testConfig;

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesAPIType.ESignature);
        }

        [Fact]
        public void SigningViaEmail_CorrectInputParameters_ReturnEnvelopeId()
        {
            // Arrange
            string docPdf = _testConfig.PathToSolution + PDF_DOCUMENT_NAME;
            string docDocx = _testConfig.PathToSolution + DOCX_DOCUMENT_NAME;
            string basePath = _testConfig.BasePath + REST_API_PREFIX;
            string envelopeStatus = "sent";

            // Act
            string envelopeId = SigningViaEmail.SendEnvelopeViaEmail(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                CC_MAIL,
                CC_NAME,
                _testConfig.AccessToken,
                basePath,
                _testConfig.AccountId,
                docDocx,
                docPdf,
                envelopeStatus);

            // Assert
            Assert.NotNull(envelopeId);
        }

        [Fact]
        public void MakeEnvelopeWithCC_CorrectInputParameters_ReturnsEnvelopeDefinition()
        {
            // Arrange
            var docPdf = _testConfig.PathToSolution + PDF_DOCUMENT_NAME;
            var docDocx = _testConfig.PathToSolution + DOCX_DOCUMENT_NAME;
            var envelopeStatus = "sent";
            var htmlFileExtension = "html";
            var docxFileExtension = "docx";
            var pdfFileExtension = "pdf";
            var anchorUnits = "pixels";
            var anchorYOffset = "10";
            var anchorXOffset = "20";

            var expectedEnvelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = "Please sign this document set",
                Documents = new List<Document>
                {
                    new Document
                    {
                        DocumentBase64 = Convert.ToBase64String(SigningViaEmail.Document1(_testConfig.SignerEmail, _testConfig.SignerName, CC_MAIL, CC_NAME)),
                        Name = "Order acknowledgement",
                        FileExtension = htmlFileExtension,
                        DocumentId = "1"
                    },
                    new Document
                    {
                        DocumentBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(docDocx)),
                        Name = "Battle Plan",
                        FileExtension = docxFileExtension,
                        DocumentId = "2",
                    },
                    new Document
                    {
                        DocumentBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(docPdf)),
                        Name = "Lorem Ipsum",
                        FileExtension = pdfFileExtension,
                        DocumentId = "3",
                    }
                },
                Status = envelopeStatus,
                Recipients = new Recipients
                {
                    Signers = new List<Signer>
                    {
                        new Signer
                        {
                            Email = _testConfig.SignerEmail,
                            Name = _testConfig.SignerName,
                            RecipientId = "1",
                            RoutingOrder = "1",
                            Tabs = new Tabs
                            {
                                SignHereTabs = new List<SignHere>
                                {
                                    new SignHere
                                    {
                                        AnchorString = "**signature_1**",
                                        AnchorUnits = anchorUnits,
                                        AnchorYOffset = anchorYOffset,
                                        AnchorXOffset = anchorXOffset,
                                    },
                                    new SignHere
                                    {
                                        AnchorString = "/sn1/",
                                        AnchorUnits = anchorUnits,
                                        AnchorYOffset = anchorYOffset,
                                        AnchorXOffset = anchorXOffset,
                                    }
                                }
                            }
                        }
                    },
                    CarbonCopies = new List<CarbonCopy>
                    {
                        new CarbonCopy
                        {
                            Email = CC_MAIL,
                            Name = CC_NAME,
                            RecipientId = "2",
                            RoutingOrder = "2",
                        }
                    },
                }
            };

            // Act
            EnvelopeDefinition envelopeDefinition = SigningViaEmail.MakeEnvelope(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                CC_MAIL,
                CC_NAME,
                docDocx,
                docPdf,
                envelopeStatus
            );

            // Assert
            Assert.NotNull(envelopeDefinition);
            envelopeDefinition.Should().BeEquivalentTo(expectedEnvelopeDefinition);
        }
    }
}
