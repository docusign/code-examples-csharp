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
    public sealed class UseTemplateUnitTests
    {
        private readonly ITestConfig _testConfig;

        private readonly CreateNewTemplateUnitTests _createNewTemplateUnitTests = new CreateNewTemplateUnitTests();

        public UseTemplateUnitTests() : this(TestConfig.Instance) { }

        private UseTemplateUnitTests(ITestConfig testConfig)
        {
            this._testConfig = testConfig;

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesAPIType.ESignature);
        }

        [Fact]
        public void UseTemplate_CorrectInputParameters_ReturnsEnvelopeId()
        {
            // Arrange
            string basePath = _testConfig.BasePath + "/restapi";
            string ccEmail = "cc@gmail.com";
            string ccName = "CC";

            _createNewTemplateUnitTests.CreateNewTemplate_CorrectInputParameters_ReturnsTemplateIdAndName();

            // Act
            string envelopeId = CreateEnvelopeFromTemplate.SendEnvelopeFromTemplate(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                ccEmail,
                ccName,
                _testConfig.AccessToken,
                basePath,
                _testConfig.AccountId,
                _testConfig.TemplateId);

            // Assert
            Assert.NotNull(envelopeId);
        }

        [Fact]
        public void MakeEnvelopeUsingTemplate_CorrectInputParameters_ReturnsEnvelopeDefinition()
        {
            // Arrange
            _createNewTemplateUnitTests.CreateNewTemplate_CorrectInputParameters_ReturnsTemplateIdAndName();

            string ccEmail = "cc@gmail.com";
            string ccName = "CC";
            var status = "sent";
            var signerRoleName = "signer";
            var ccRoleName = "cc";

            var expectedEnvelopeDefinition = new EnvelopeDefinition
            {
                Status = status,
                TemplateRoles = new List<TemplateRole>
                {
                    new TemplateRole
                    {
                        Email = _testConfig.SignerEmail,
                        Name = _testConfig.SignerName,
                        RoleName = signerRoleName
                    },
                    new TemplateRole
                    {
                        Email = ccEmail,
                        Name = ccName,
                        RoleName = ccRoleName
                    }
                },
                TemplateId = _testConfig.TemplateId
            };

            // Act
            EnvelopeDefinition envelopeDefinition = CreateEnvelopeFromTemplate.MakeEnvelope(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                ccEmail,
                ccName,
                _testConfig.TemplateId
            );

            // Assert
            Assert.NotNull(envelopeDefinition);
            envelopeDefinition.Should().BeEquivalentTo(expectedEnvelopeDefinition);
        }
    }
}
