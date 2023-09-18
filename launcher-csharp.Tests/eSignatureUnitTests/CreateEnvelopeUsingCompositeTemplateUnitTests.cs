using System;
using System.Collections.Generic;
using DocuSign.CodeExamples.Common;
using DocuSign.eSign.Model;
using ESignature.Examples;
using FluentAssertions;
using Xunit;

namespace launcher_csharp.Tests.eSignatureUnitTests
{
    [Collection("eSignature tests")]
    public sealed class CreateEnvelopeUsingCompositeTemplateUnitTests
    {
        private const string RedirectUrl = "https://developers.docusign.com/docs/esign-rest-api/";

        private const string RestApiPrefix = "/restapi";

        private const string CcEmail = "cc@gmail.com";

        private const string CcName = "CC";

        private const string Item = "avocado";

        private const string Quantity = "1";

        private const string SignerClientId = "1000";

        private readonly TestConfig _testConfig;

        private readonly CreateNewTemplateUnitTests _createNewTemplateUnitTests;

        public CreateEnvelopeUsingCompositeTemplateUnitTests()
        {
            this._testConfig = new TestConfig();

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesApiType.ESignature, _testConfig);
            this._createNewTemplateUnitTests = new CreateNewTemplateUnitTests(_testConfig);
        }

        [Fact]
        public void CreateEnvelopeUsingCompositeTemplate_CorrectInputParameters_ReturnsRedirectURL()
        {
            // Arrange
            _createNewTemplateUnitTests.CreateNewTemplate_CorrectInputParameters_ReturnsTemplateIdAndName();

            string basePath = _testConfig.BasePath + RestApiPrefix;

            // Act
            string redirectUrl = CreateEnvelopeUsingCompositeTemplate.CreateEnvelopeFromCompositeTemplate(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                CcEmail,
                CcName,
                _testConfig.AccessToken,
                basePath,
                _testConfig.AccountId,
                Item,
                Quantity,
                RedirectUrl,
                SignerClientId,
                _testConfig.TemplateId);

            // Assert
            Assert.NotNull(redirectUrl);
        }

        [Fact]
        public void MakeRecipientViewRequestForCompositeTemplate_CorrectInputParameters_ReturnsViewRequest()
        {
            // Arrange
            var authenticationMethod = "none";

            var expectedRecipientViewRequest = new RecipientViewRequest
            {
                ReturnUrl = RedirectUrl,
                AuthenticationMethod = authenticationMethod,
                Email = _testConfig.SignerEmail,
                UserName = _testConfig.SignerName,
                ClientUserId = _testConfig.ImpersonatedUserId
            };

            // Act
            RecipientViewRequest recipientViewRequest = CreateEnvelopeUsingCompositeTemplate.MakeRecipientViewRequest(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                RedirectUrl,
                _testConfig.ImpersonatedUserId
            );

            // Assert
            Assert.NotNull(recipientViewRequest);
            recipientViewRequest.Should().BeEquivalentTo(expectedRecipientViewRequest);
        }

        [Fact]
        public void MakeEnvelopeUsingCompositeTemplate_CorrectInputParameters_ReturnsEnvelopeDefinition()
        {
            // Arrange
            _createNewTemplateUnitTests.CreateNewTemplate_CorrectInputParameters_ReturnsTemplateIdAndName();

            var signerRole = "signer";
            var ccRole = "cc";
            var envelopeStatus = "sent";
            var anchorString = "**signature_1**";
            var anchorYOffset = "10";
            var anchorUnits = "pixels";
            var anchorXOffset = "20";
            var fileExtension = "html";
            var fileName = "Appendix 1--Sales order";

            var carbonCopy = new CarbonCopy
            {
                Email = CcEmail,
                Name = CcName,
                RoleName = ccRole,
                RecipientId = "2"
            };

            var signer = new Signer
            {
                Email = _testConfig.SignerEmail,
                Name = _testConfig.SignerName,
                ClientUserId = SignerClientId,
                RoleName = signerRole,
                RecipientId = "1",
                Tabs = new Tabs
                {
                    SignHereTabs = new List<SignHere>
                    {
                        new SignHere
                        {
                            AnchorString = anchorString,
                            AnchorYOffset = anchorYOffset,
                            AnchorUnits = anchorUnits,
                            AnchorXOffset = anchorXOffset
                        }
                    }
                }
            };

            var expectedEnvelopeDefinition = new EnvelopeDefinition
            {
                Status = envelopeStatus,
                CompositeTemplates = new List<CompositeTemplate>
                {
                    new CompositeTemplate
                    {
                        CompositeTemplateId = "1",
                        ServerTemplates = new List<ServerTemplate>
                        {
                            new ServerTemplate
                            {
                                Sequence = "1",
                                TemplateId = _testConfig.TemplateId
                            }
                        },
                        InlineTemplates = new List<InlineTemplate>
                        {
                            new InlineTemplate
                            {
                                Sequence = "2",
                                Recipients = new Recipients
                                {
                                    Signers = new List<Signer>
                                    {
                                        new Signer
                                        {
                                            Email = _testConfig.SignerEmail,
                                            Name = _testConfig.SignerName,
                                            ClientUserId = SignerClientId,
                                            RoleName = signerRole,
                                            RecipientId = "1"
                                        }
                                    },
                                    CarbonCopies = new List<CarbonCopy> { carbonCopy }
                                }
                            }
                        }
                    },
                    new CompositeTemplate
                    {
                        CompositeTemplateId = "2",
                        Document = new Document
                        {
                            DocumentBase64 = Convert.ToBase64String(CreateEnvelopeUsingCompositeTemplate.Document1(_testConfig.SignerEmail,
                                _testConfig.SignerName, CcEmail, CcName, Item, Quantity)),
                            Name = fileName,
                            FileExtension = fileExtension,
                            DocumentId = "1"
                        },
                        InlineTemplates = new List<InlineTemplate>
                        {
                            new InlineTemplate
                            {
                                Sequence = "1",
                                Recipients = new Recipients
                                {
                                    Signers = new List<Signer> { signer },
                                    CarbonCopies = new List<CarbonCopy> { carbonCopy}
                                },
                            }
                        }
                    }
                }
            };

            // Act
            EnvelopeDefinition envelopeDefinition = CreateEnvelopeUsingCompositeTemplate.MakeEnvelope(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                CcEmail,
                CcName,
                Item,
                Quantity,
                SignerClientId,
                _testConfig.TemplateId
            );

            // Assert
            Assert.NotNull(envelopeDefinition);
            envelopeDefinition.Should().BeEquivalentTo(expectedEnvelopeDefinition);
        }
    }
}
