// <copyright file="DSConfiguration.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Models
{
    public class DsConfiguration
    {
        public string RedirectForEg043 = "Eg043/EnvelopesListStatus";
        public string DocDocx = "World_Wide_Corp_Battle_Plan_Trafalgar.docx";
        public string TabsDocx = "World_Wide_Corp_salary.docx";
        public string DocPdf = "World_Wide_Corp_lorem.pdf";
        public string OfferDocDocx = "Offer_Letter_Demo.docx";
        public string DocCsv = "UserData.csv";
        public string DocHtml = "doc_1.html";
        public string ExportUsersPath = @"..\..\..\ExportedUserData.csv";
        public string GithubExampleUrl = "https://github.com/docusign/code-examples-csharp/blob/master/launcher-csharp";
        public string Documentation = null;

        public string AppUrl { get; set; }

        public string SignerEmail { get; set; }

        public string SignerName { get; set; }

        public string GatewayAccountId { get; set; }

        public string GatewayName { get; set; }

        public string QuickAcg { get; set; }

        public bool IsLoggedInAfterEg043 { get; set; }

        public string GatewayDisplayName { get; set; }

        public string CodeExamplesManifest { get; set; }

        public string PrincipalUserId { get; set; }
    }
}