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
    public sealed class CreateEnvelopeUsingCompositeTemplateUnitTests
    {
        private const string RedirectUrl = "https://developers.docusign.com/docs/esign-rest-api/";

        private readonly ITestConfig _testConfig;

        private readonly CreateNewTemplateUnitTests _createNewTemplateUnitTests = new CreateNewTemplateUnitTests();

        public CreateEnvelopeUsingCompositeTemplateUnitTests() : this(TestConfig.Instance) { }

        private CreateEnvelopeUsingCompositeTemplateUnitTests(ITestConfig testConfig)
        {
            this._testConfig = testConfig;

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesAPIType.ESignature);
        }

        [Fact]
        public void CreateEnvelopeUsingCompositeTemplate_CorrectInputParameters_ReturnsRedirectURL()
        {
            // Arrange
            _createNewTemplateUnitTests.CreateNewTemplate_CorrectInputParameters_ReturnsTemplateIdAndName();

            string basePath = _testConfig.BasePath + "/restapi";
            string ccEmail = "cc@gmail.com";
            string ccName = "CC";
            string item = "avocado";
            string quantity = "1";
            string signerClientId = "1000";

            // Act
            string redirectUrl = CreateEnvelopeUsingCompositeTemplate.CreateEnvelopeFromCompositeTemplate(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                ccEmail,
                ccName,
                _testConfig.AccessToken,
                basePath,
                _testConfig.AccountId,
                item,
                quantity,
                RedirectUrl,
                signerClientId,
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

            var ccEmail = "cc@gmail.com";
            var ccName = "CC";
            var item = "avocado";
            var quantity = "1";
            var signerClientId = "1000";
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
                Email = ccEmail,
                Name = ccName,
                RoleName = ccRole,
                RecipientId = "2"
            };

            var signer = new Signer
            {
                Email = _testConfig.SignerEmail,
                Name = _testConfig.SignerName,
                ClientUserId = signerClientId,
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
                                            ClientUserId = signerClientId,
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
                                _testConfig.SignerName, ccEmail, ccName, item, quantity)),
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
                ccEmail,
                ccName,
                item,
                quantity,
                signerClientId,
                _testConfig.TemplateId
            );

            // Assert
            Assert.NotNull(envelopeDefinition);
            envelopeDefinition.Should().BeEquivalentTo(expectedEnvelopeDefinition);
        }
    }
}
