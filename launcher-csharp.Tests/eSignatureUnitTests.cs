using Xunit;
using DocuSign.CodeExamples.Common;
using DocuSign.eSign.Model;
using ESignature.Examples;

namespace launcher_csharp.Tests
{
    public sealed class ESignatureUnitTests
    {
        private const string RedirectUrl = "https://developers.docusign.com/docs/esign-rest-api/";

        private readonly ITestConfig _testConfig;

        public ESignatureUnitTests() : this(TestConfig.Instance) { }

        private ESignatureUnitTests(ITestConfig testConfig)
        {
            this._testConfig = testConfig;

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesAPIType.ESignature);
        }

        #region EmbeddedSigningCeremony
        [Fact]
        public void EmbeddedSigningCeremony_CorrectInputParameters_ReturnsEnvelopeIdAndRedirectUrl()
        {
            string signerEmail = _testConfig.SignerEmail;
            string signerName = _testConfig.SignerName;
            string signerClientId = _testConfig.ImpersonatedUserId;
            string accountId = _testConfig.AccountId;
            string accessToken = _testConfig.AccessToken;
            string docPdf = _testConfig.PathToSolution + "World_Wide_Corp_lorem.pdf";
            string basePath = _testConfig.BasePath + "/restapi";

            (string, string) result = EmbeddedSigningCeremony.SendEnvelopeForEmbeddedSigning(
                signerEmail,
                signerName,
                signerClientId,
                accessToken,
                basePath,
                accountId,
                docPdf,
                RedirectUrl,
                RedirectUrl
            );

            Assert.NotNull(result.Item1);
            Assert.NotNull(result.Item2);
        }

        [Fact]
        public void MakeRecipientViewRequest_CorrectInputParameters_ReturnsViewRequest()
        {
            string signerEmail = _testConfig.SignerEmail;
            string signerName = _testConfig.SignerName;
            string signerClientId = _testConfig.ImpersonatedUserId;

            RecipientViewRequest recipientViewRequest = EmbeddedSigningCeremony.MakeRecipientViewRequest(
                signerEmail,
                signerName,
                RedirectUrl,
                signerClientId,
                RedirectUrl
            );

            Assert.NotNull(recipientViewRequest);
            Assert.NotNull(recipientViewRequest.ReturnUrl);
            Assert.NotNull(recipientViewRequest.AuthenticationMethod);
            Assert.NotNull(recipientViewRequest.Email);
            Assert.NotNull(recipientViewRequest.UserName);
            Assert.NotNull(recipientViewRequest.ClientUserId);
            Assert.NotNull(recipientViewRequest.PingFrequency);
            Assert.NotNull(recipientViewRequest.PingUrl);
        }

        [Fact]
        public void MakeEnvelope_CorrectInputParameters_ReturnsEnvelopeDefinition()
        {
            string signerEmail = _testConfig.SignerEmail;
            string signerName = _testConfig.SignerName;
            string signerClientId = _testConfig.ImpersonatedUserId;
            string docPdf = _testConfig.PathToSolution + "World_Wide_Corp_lorem.pdf";

            EnvelopeDefinition envelopeDefinition = EmbeddedSigningCeremony.MakeEnvelope(
                signerEmail,
                signerName,
                signerClientId,
                docPdf
            );

            Assert.NotNull(envelopeDefinition);
            Assert.NotNull(envelopeDefinition.EmailSubject);
            Assert.NotNull(envelopeDefinition.Recipients);
            Assert.NotEmpty(envelopeDefinition.Documents);
            Assert.Single(envelopeDefinition.Documents);
            Assert.NotEmpty(envelopeDefinition.Recipients.Signers);
            Assert.Single(envelopeDefinition.Recipients.Signers);
            Assert.NotNull(envelopeDefinition.Status);
        }
        #endregion

        #region SigningViaEmail
        [Fact]
        public void SigningViaEmail_CorrectInputParameters_ReturnEnvelopeId()
        {
            string docPdf = _testConfig.PathToSolution + "World_Wide_Corp_lorem.pdf";
            string docDocx = _testConfig.PathToSolution + "World_Wide_Corp_Battle_Plan_Trafalgar.docx";
            string basePath = _testConfig.BasePath + "/restapi";
            string signerEmail = _testConfig.SignerEmail;
            string signerName = _testConfig.SignerName;
            string ccEmail = "cc@gmail.com";
            string ccName = "CC";
            string envelopeStatus = "sent";
            string accessToken = _testConfig.AccessToken;
            string accountId = _testConfig.AccountId;

            string envelopeId = SigningViaEmail.SendEnvelopeViaEmail(
                signerEmail,
                signerName,
                ccEmail,
                ccName,
                accessToken,
                basePath,
                accountId,
                docDocx,
                docPdf,
                envelopeStatus);

            Assert.NotNull(envelopeId);
        }

        [Fact]
        public void MakeEnvelopeWithCC_CorrectInputParameters_ReturnsEnvelopeDefinition()
        {
            string signerEmail = _testConfig.SignerEmail;
            string signerName = _testConfig.SignerName;
            string ccEmail = "cc@gmail.com";
            string ccName = "CC";
            string docPdf = _testConfig.PathToSolution + "World_Wide_Corp_lorem.pdf";
            string docDocx = _testConfig.PathToSolution + "World_Wide_Corp_Battle_Plan_Trafalgar.docx";
            string envelopeStatus = "sent";

            EnvelopeDefinition envelopeDefinition = SigningViaEmail.MakeEnvelope(
                signerEmail,
                signerName,
                ccEmail,
                ccName,
                docDocx,
                docPdf,
                envelopeStatus
            );

            Assert.NotNull(envelopeDefinition);
            Assert.NotNull(envelopeDefinition.EmailSubject);
            Assert.NotNull(envelopeDefinition.Recipients);
            Assert.NotEmpty(envelopeDefinition.Documents);
            Assert.Equal(3, envelopeDefinition.Documents.Count);
            Assert.NotEmpty(envelopeDefinition.Recipients.Signers);
            Assert.NotEmpty(envelopeDefinition.Recipients.CarbonCopies);
            Assert.Single(envelopeDefinition.Recipients.Signers);
            Assert.Single(envelopeDefinition.Recipients.CarbonCopies);
            Assert.NotNull(envelopeDefinition.Status);
        }
        #endregion

        #region CreateNewTemplate
        [Fact]
        public void CreateNewTemplate_CorrectInputParameters_ReturnsTemplateIdAndName()
        {
            string basePath = _testConfig.BasePath + "/restapi";
            string accessToken = _testConfig.AccessToken;
            string accountId = _testConfig.AccountId;
            string docPdf = _testConfig.PathToSolution + "World_Wide_Corp_lorem.pdf";

            (bool createdNewTemplate, string templateId, string resultsTemplateName) = CreateNewTemplate.CreateTemplate(
                accessToken,
                basePath,
                accountId,
                docPdf);

            _testConfig.TemplateId = templateId;

            Assert.NotNull(createdNewTemplate);
            Assert.NotNull(templateId);
            Assert.NotNull(resultsTemplateName);
        }

        [Fact]
        public void MakeTemplate_CorrectInputParameters_ReturnsEnvelopeTemplate()
        {
            string templateName = "Test template";
            string docPdf = _testConfig.PathToSolution + "World_Wide_Corp_lorem.pdf";

            EnvelopeTemplate envelopeTemplate = CreateNewTemplate.MakeTemplate(templateName, docPdf);

            Assert.NotNull(envelopeTemplate);
            Assert.NotNull(envelopeTemplate.Description);
            Assert.Equal(templateName, envelopeTemplate.Name);
            Assert.NotNull(envelopeTemplate.Documents);
            Assert.Single(envelopeTemplate.Documents);
            Assert.NotEmpty(envelopeTemplate.EmailSubject);
            Assert.NotEmpty(envelopeTemplate.Recipients.CarbonCopies);
            Assert.NotEmpty(envelopeTemplate.Recipients.Signers);
            Assert.Single(envelopeTemplate.Recipients.CarbonCopies);
            Assert.Single(envelopeTemplate.Recipients.Signers);
            Assert.NotNull(envelopeTemplate.Status);
        }
        #endregion

        #region UseTemplate
        [Fact]
        public void UseTemplate_CorrectInputParameters_ReturnsEnvelopeId()
        {
            string basePath = _testConfig.BasePath + "/restapi";
            string ccEmail = "cc@gmail.com";
            string ccName = "CC";
            string signerEmail = "cc@gmail.com";
            string signerName = _testConfig.SignerName;
            string accessToken = _testConfig.AccessToken;
            string accountId = _testConfig.AccountId;
            string templateId = _testConfig.TemplateId;

            CreateNewTemplate_CorrectInputParameters_ReturnsTemplateIdAndName();

            string envelopeId = CreateEnvelopeFromTemplate.SendEnvelopeFromTemplate(
                signerEmail,
                signerName,
                ccEmail,
                ccName,
                accessToken,
                basePath,
                accountId,
                templateId);

            Assert.NotNull(envelopeId);
        }

        [Fact]
        public void MakeEnvelopeUsingTemplate_CorrectInputParameters_ReturnsEnvelopeDefinition()
        {
            CreateNewTemplate_CorrectInputParameters_ReturnsTemplateIdAndName();

            string ccEmail = "cc@gmail.com";
            string ccName = "CC";
            string signerEmail = _testConfig.SignerEmail;
            string signerName = _testConfig.SignerName;
            string templateId = _testConfig.TemplateId;

            EnvelopeDefinition envelopeDefinition = CreateEnvelopeFromTemplate.MakeEnvelope(
                signerEmail,
                signerName,
                ccEmail,
                ccName,
                templateId
            );

            Assert.NotNull(envelopeDefinition);
            Assert.NotNull(envelopeDefinition.TemplateId);
            Assert.NotNull(envelopeDefinition.Status);
            Assert.NotEmpty(envelopeDefinition.TemplateRoles);
        }
        #endregion

        #region CreateEnvelopeUsingCompositeTemplate
        [Fact]
        public void CreateEnvelopeUsingCompositeTemplate_CorrectInputParameters_ReturnsRedirectURL()
        {
            CreateNewTemplate_CorrectInputParameters_ReturnsTemplateIdAndName();

            string basePath = _testConfig.BasePath + "/restapi";
            string ccEmail = "cc@gmail.com";
            string ccName = "CC";
            string signerEmail = _testConfig.SignerEmail;
            string signerName = _testConfig.SignerName;
            string item = "avocado";
            string quantity = "1";
            string signerClientId = "1000";
            string accessToken = _testConfig.AccessToken;
            string accountId = _testConfig.AccountId;
            string templateId = _testConfig.TemplateId;

            string redirectUrl = CreateEnvelopeUsingCompositeTemplate.CreateEnvelopeFromCompositeTemplate(
                signerEmail,
                signerName,
                ccEmail,
                ccName,
                accessToken,
                basePath,
                accountId,
                item,
                quantity,
                RedirectUrl,
                signerClientId,
                templateId);

            Assert.NotNull(redirectUrl);
        }

        [Fact]
        public void MakeRecipientViewRequestForCompositeTemplate_CorrectInputParameters_ReturnsViewRequest()
        {
            string signerEmail = _testConfig.SignerEmail;
            string signerName = _testConfig.SignerName;
            string signerClientId = _testConfig.ImpersonatedUserId;

            RecipientViewRequest recipientViewRequest = CreateEnvelopeUsingCompositeTemplate.MakeRecipientViewRequest(
                signerEmail,
                signerName,
                RedirectUrl,
                signerClientId
            );

            Assert.NotNull(recipientViewRequest);
            Assert.NotNull(recipientViewRequest.ReturnUrl);
            Assert.NotNull(recipientViewRequest.AuthenticationMethod);
            Assert.NotNull(recipientViewRequest.Email);
            Assert.NotNull(recipientViewRequest.UserName);
            Assert.NotNull(recipientViewRequest.ClientUserId);
        }

        [Fact]
        public void MakeEnvelopeUsingCompositeTemplate_CorrectInputParameters_ReturnsEnvelopeDefinition()
        {
            CreateNewTemplate_CorrectInputParameters_ReturnsTemplateIdAndName();

            string ccEmail = "cc@gmail.com";
            string ccName = "CC";
            string signerEmail = _testConfig.SignerEmail;
            string signerName = _testConfig.SignerName;
            string templateId = _testConfig.TemplateId;
            string item = "avocado";
            string quantity = "1";
            string signerClientId = "1000";

            EnvelopeDefinition envelopeDefinition = CreateEnvelopeUsingCompositeTemplate.MakeEnvelope(
                signerEmail,
                signerName,
                ccEmail,
                ccName,
                item,
                quantity,
                signerClientId,
                templateId
            );

            Assert.NotNull(envelopeDefinition);
            Assert.NotNull(envelopeDefinition.Status);
            Assert.NotEmpty(envelopeDefinition.CompositeTemplates);
            Assert.Equal(2, envelopeDefinition.CompositeTemplates.Count);
        }
        #endregion

        #region SetEnvelopeTabValue
        [Fact]
        public void SetEnvelopeTabValue_CorrectInputParameters_ReturnsEnvelopeIdAndRedirectURL()
        {
            string basePath = _testConfig.BasePath + "/restapi";
            string tabsDocx = _testConfig.PathToSolution + "World_Wide_Corp_salary.docx";
            string signerName = _testConfig.SignerName;
            string signerEmail = _testConfig.SignerEmail;
            string signerClientId = _testConfig.ImpersonatedUserId;
            string accessToken = _testConfig.AccessToken;
            string accountId = _testConfig.AccountId;

            (string envelopeId, string redirectUrl) = SetEnvelopeTabValue.CreateEnvelopeAndUpdateTabData(
                signerEmail,
                signerName,
                signerClientId,
                accessToken,
                basePath,
                accountId,
                tabsDocx,
                RedirectUrl,
                RedirectUrl);

            Assert.NotNull(redirectUrl);
            Assert.NotNull(envelopeId);
        }
        #endregion
    }
}
