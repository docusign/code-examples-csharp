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
        private const string REST_API_PREFIX = "/restapi";

        private const string CC_MAIL = "cc@gmail.com";

        private const string CC_NAME = "CC";

        private readonly ITestConfig _testConfig;

        private readonly CreateNewTemplateUnitTests _createNewTemplateUnitTests;

        public UseTemplateUnitTests()
        {
            this._testConfig = new TestConfig();

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken(ExamplesAPIType.ESignature, _testConfig);
            this._createNewTemplateUnitTests = new CreateNewTemplateUnitTests(_testConfig);
        }

        [Fact]
        public void UseTemplate_CorrectInputParameters_ReturnsEnvelopeId()
        {
            // Arrange
            string basePath = _testConfig.BasePath + REST_API_PREFIX;

            _createNewTemplateUnitTests.CreateNewTemplate_CorrectInputParameters_ReturnsTemplateIdAndName();

            // Act
            string envelopeId = CreateEnvelopeFromTemplate.SendEnvelopeFromTemplate(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                CC_MAIL,
                CC_NAME,
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
                        Email = CC_MAIL,
                        Name = CC_NAME,
                        RoleName = ccRoleName
                    }
                },
                TemplateId = _testConfig.TemplateId
            };

            // Act
            EnvelopeDefinition envelopeDefinition = CreateEnvelopeFromTemplate.MakeEnvelope(
                _testConfig.SignerEmail,
                _testConfig.SignerName,
                CC_MAIL,
                CC_NAME,
                _testConfig.TemplateId
            );

            // Assert
            Assert.NotNull(envelopeDefinition);
            envelopeDefinition.Should().BeEquivalentTo(expectedEnvelopeDefinition);
        }
    }
}
