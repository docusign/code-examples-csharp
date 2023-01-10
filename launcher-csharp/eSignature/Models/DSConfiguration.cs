// <copyright file="DSConfiguration.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Models
{
    public class DSConfiguration
    {
        public string AppUrl { get; set; }

        public string SignerEmail { get; set; }

        public string SignerName { get; set; }

        public string GatewayAccountId { get; set; }

        public string GatewayName { get; set; }

        public string QuickACG { get; set; }

        public string GatewayDisplayName { get; set; }

        public string CodeExamplesManifest { get; set; }

        public bool Production = false;
        public bool Debug = true; // Send debugging statements to console
        public string SessionSecret = "12345"; // Secret for encrypting session cookie content
        public bool AllowSilentAuthentication = true; // a user can be silently authenticated if they have an

        // active login session on another tab of the same browser
        // Set if you want a specific DocuSign AccountId, If null, the user's default account will be used.
        public string TargetAccountId = null;
        public string DemoDocPath = "demo_documents";
        public string DocDocx = "World_Wide_Corp_Battle_Plan_Trafalgar.docx";
        public string TabsDocx = "World_Wide_Corp_salary.docx";
        public string DocPdf = "World_Wide_Corp_lorem.pdf";
        public string DocCsv = "UserData.csv";
        public string DocHTML = "doc_1.html";
        public string ExportUsersPath = @"..\..\..\ExportedUserData.csv";
        public string GithubExampleUrl = "https://github.com/docusign/code-examples-csharp/blob/master/launcher-csharp";
        public string Documentation = null;
    }
}