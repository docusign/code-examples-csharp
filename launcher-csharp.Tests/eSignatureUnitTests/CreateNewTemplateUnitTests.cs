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
    public sealed class CreateNewTemplateUnitTests
    {
        private const string RestApiPrefix = "/restapi";

        private const string PdfDocumentName = "World_Wide_Corp_lorem.pdf";

        private readonly TestConfig _testConfig;

        public CreateNewTemplateUnitTests(TestConfig testConfig = null)
        {
            this._testConfig = testConfig ?? new TestConfig();

            var jwtLoginMethod = new JwtLoginMethodUnitTest();
            jwtLoginMethod.RequestJWTUserToken_CorrectInputParameters_ReturnsOAuthToken(ExamplesApiType.ESignature, _testConfig);
        }

        [Fact]
        public void CreateNewTemplate_CorrectInputParameters_ReturnsTemplateIdAndName()
        {
            // Arrange
            string basePath = _testConfig.BasePath + RestApiPrefix;
            string docPdf = _testConfig.PathToSolution + PdfDocumentName;

            //Act
            var template = CreateNewTemplate.CreateTemplate(
                _testConfig.AccessToken,
                basePath,
                _testConfig.AccountId,
                docPdf);

            string templateId = template.templateId;
            bool createdNewTemplate = template.createdNewTemplate;
            string templateName = template.resultsTemplateName;

            _testConfig.TemplateId = templateId;

            // Assert
            Assert.NotNull(templateId);
            Assert.NotNull(createdNewTemplate);
            Assert.NotNull(templateName);
        }

        [Fact]
        public void MakeTemplate_CorrectInputParameters_ReturnsEnvelopeTemplate()
        {
            // Arrange
            string templateName = "Test template";
            string docPdf = _testConfig.PathToSolution + PdfDocumentName;
            var envelopeStatus = "created";
            var emailSubject = "Please sign this document";
            var emailDescription = "Example template created via the API";
            var roleName = "cc";
            var documentName = "Lorem Ipsum";
            var signer = "signer";

            var expectedEnvelopeTemplate = new EnvelopeTemplate
            {
                Status = envelopeStatus,
                Recipients = new Recipients
                {
                    Signers = new List<Signer>
                    {
                        new Signer
                        {
                            RoleName = signer,
                            RecipientId = "1",
                            RoutingOrder = "1",
                            Tabs = new Tabs
                            {
                                SignHereTabs = new List<SignHere>
                                {
                                    new SignHere
                                    {
                                        DocumentId = "1",
                                        PageNumber = "1",
                                        XPosition = "191",
                                        YPosition = "148"
                                    }
                                },
                                ListTabs = new List<List>
                                {
                                    new List
                                    {
                                        DocumentId = "1",
                                        PageNumber = "1",
                                        XPosition = "142",
                                        YPosition = "291",
                                        Font = "helvetica",
                                        FontSize = "size14",
                                        TabLabel = "list",
                                        Required = "false",
                                        ListItems = new List<ListItem>
                                        {
                                            new ListItem { Text = "Red", Value = "Red" },
                                            new ListItem { Text = "Orange", Value = "Orange" },
                                            new ListItem { Text = "Yellow", Value = "Yellow" },
                                            new ListItem { Text = "Green", Value = "Green" },
                                            new ListItem { Text = "Blue", Value = "Blue" },
                                            new ListItem { Text = "Indigo", Value = "Indigo" },
                                            new ListItem { Text = "Violet", Value = "Violet" },
                                        }
                                    }
                                },
                                CheckboxTabs = new List<Checkbox>
                                {
                                    new Checkbox
                                    {
                                        DocumentId = "1",
                                        PageNumber = "1",
                                        XPosition = "75",
                                        YPosition = "417",
                                        TabLabel = "ckAuthorization"
                                    },
                                    new Checkbox
                                    {
                                        DocumentId = "1",
                                        PageNumber = "1",
                                        XPosition = "75",
                                        YPosition = "447",
                                        TabLabel = "ckAuthentication"
                                    },
                                    new Checkbox
                                    {
                                        DocumentId = "1",
                                        PageNumber = "1",
                                        XPosition = "75",
                                        YPosition = "478",
                                        TabLabel = "ckAgreement"
                                    },
                                    new Checkbox
                                    {
                                        DocumentId = "1",
                                        PageNumber = "1",
                                        XPosition = "75",
                                        YPosition = "508",
                                        TabLabel = "ckAcknowledgement"
                                    }
                                },
                                RadioGroupTabs = new List<RadioGroup>
                                {
                                    new RadioGroup
                                    {
                                        DocumentId = "1",
                                        GroupName = "radio1",
                                        Radios = new List<Radio>
                                        {
                                            new Radio { PageNumber = "1", Value = "white", XPosition = "142", YPosition = "384", Required = "false" },
                                            new Radio { PageNumber = "1", Value = "red", XPosition = "74", YPosition = "384", Required = "false" },
                                            new Radio { PageNumber = "1", Value = "blue", XPosition = "220", YPosition = "384", Required = "false" },
                                        }
                                    }
                                },
                                TextTabs = new List<Text>
                                {
                                    new Text
                                    {
                                        DocumentId = "1",
                                        PageNumber = "1",
                                        XPosition = "153",
                                        YPosition = "230",
                                        Font = "helvetica",
                                        FontSize = "size14",
                                        TabLabel = "text",
                                        Height = "23",
                                        Width = "84",
                                        Required = "false"
                                    },
                                }
                            }
                        }
                    },
                    CarbonCopies = new List<CarbonCopy>
                    {
                        new CarbonCopy
                        {
                            RoleName = roleName,
                            RoutingOrder = "2",
                            RecipientId = "2"
                        }
                    }
                },
                EmailSubject = emailSubject,
                Documents = new List<Document>
                {
                    new Document
                    {
                        DocumentBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(docPdf)),
                        Name = documentName,
                        FileExtension = "pdf",
                        DocumentId = "1"
                    }
                },
                Name = templateName,
                Description = emailDescription
            };

            // Act
            EnvelopeTemplate envelopeTemplate = CreateNewTemplate.MakeTemplate(templateName, docPdf);

            // Assert
            Assert.NotNull(envelopeTemplate);
            envelopeTemplate.Recipients.Signers[0].Tabs.TextTabs.Should().BeEquivalentTo(
                expectedEnvelopeTemplate.Recipients.Signers[0].Tabs.TextTabs);
        }
    }
}
