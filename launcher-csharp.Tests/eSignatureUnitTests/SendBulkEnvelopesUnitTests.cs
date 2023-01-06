using System.Collections.Generic;
using DocuSign.CodeExamples.Common;
using DocuSign.eSign.Model;
using ESignature.Examples;
using FluentAssertions;
using Xunit;

namespace launcher_csharp.Tests.eSignatureUnitTests
{
    public sealed class SendBulkEnvelopesUnitTests
    {
        private const string ESIGNARURE_PATH_PREFIX = "/restapi";

        private string DOC_PDF = "World_Wide_Corp_lorem.pdf";

        private readonly ITestConfig _testConfig;

        public SendBulkEnvelopesUnitTests() : this(TestConfig.Instance) { }

        private SendBulkEnvelopesUnitTests(ITestConfig testConfig)
        {
            this._testConfig = testConfig;

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesAPIType.ESignature);
        }

        [Fact]
        public void GetStatus_CorrectInputParameters_ReturnsBulkSendBatchStatus()
        {
            // Arrange
            var carbonCopy1Email = "carbonCopy@gmail.com";
            var carbonCopy1Name = "Carbon Copy";
            var carbonCopy2Email = "carbonCopy2@gmail.com";
            var carbonCopy2Name = "Carbon Copy 2";
            var signer2Email = "signer2@gmail.com";
            var signer2Name = "Signer 2";
            var envelopeIdStamping = "true";
            var emailSubject = "Please sign this document sent from the C# SDK";

            string basePath = _testConfig.BasePath + ESIGNARURE_PATH_PREFIX;
            string docPdf = _testConfig.PathToSolution + DOC_PDF;

            // Act
            BulkSendBatchStatus envelopeSummary = SendBulkEnvelopes.GetStatus(
                _testConfig.SignerName,
                _testConfig.SignerEmail,
                carbonCopy1Name,
                carbonCopy1Email,
                signer2Name,
                signer2Email,
                carbonCopy2Name,
                carbonCopy2Email,
                _testConfig.AccessToken,
                basePath,
                _testConfig.AccountId,
                docPdf,
                envelopeIdStamping,
                emailSubject
                );

            // Assert
            Assert.NotNull(envelopeSummary);
        }

        [Fact]
        public void MakeBulkSendList_CorrectInputParameters_ReturnsBulkSendingList()
        {
            // Arrange
            var carbonCopy1Email = "carbonCopy@gmail.com";
            var carbonCopy1Name = "Carbon Copy";
            var carbonCopy2Email = "carbonCopy2@gmail.com";
            var carbonCopy2Name = "Carbon Copy 2";
            var signer2Email = "signer2@gmail.com";
            var signer2Name = "Signer 2";
            var signerRole = "signer";
            var ccRole = "cc";
            var bulkSendingGroupName = "sample.csv";

            var expectedBulkSendingList = new BulkSendingList
            {
                BulkCopies = new List<BulkSendingCopy>
                {
                    new BulkSendingCopy
                    {
                        Recipients = new List<BulkSendingCopyRecipient>
                        {
                            new BulkSendingCopyRecipient
                            {

                                Name = _testConfig.SignerName,
                                Email = _testConfig.SignerEmail,
                                RoleName = signerRole,
                            },
                            new BulkSendingCopyRecipient
                            {

                                Name = carbonCopy1Name,
                                Email = carbonCopy1Email,
                                RoleName = ccRole,
                            },
                        },
                    },
                    new BulkSendingCopy
                    {
                        Recipients = new List<BulkSendingCopyRecipient>
                        {
                            new BulkSendingCopyRecipient
                            {

                                Name = signer2Name,
                                Email = signer2Email,
                                RoleName = signerRole,
                            },
                            new BulkSendingCopyRecipient
                            {

                                Name = carbonCopy2Name,
                                Email = carbonCopy2Email,
                                RoleName = ccRole,
                            },
                        },
                    },
                },
                Name = bulkSendingGroupName,
            };

            // Act
            BulkSendingList bulkSendingList = SendBulkEnvelopes.MakeBulkSendList(
                _testConfig.SignerName,
                _testConfig.SignerEmail,
                carbonCopy1Name,
                carbonCopy1Email,
                signer2Name,
                signer2Email,
                carbonCopy2Name,
                carbonCopy2Email);

            // Assert
            Assert.NotNull(bulkSendingList);
            bulkSendingList.Should().BeEquivalentTo(expectedBulkSendingList);
        }
    }
}
