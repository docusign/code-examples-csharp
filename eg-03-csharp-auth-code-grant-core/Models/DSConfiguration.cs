namespace eg_03_csharp_auth_code_grant_core.Models
{
    public class DSConfiguration
    {        
        public string AppUrl { get; set; }

        public string SignerEmail { get; set; }

        public string SignerName { get; set; }

        public string GatewayAccountId { get; set; }

        public string GatewayName { get; set; }

        public string GatewayDisplayName { get; set; }

        public bool production = false;
        public bool debug = true; // Send debugging statements to console
        public string sessionSecret = "12345"; // Secret for encrypting session cookie content
        public bool allowSilentAuthentication = true; // a user can be silently authenticated if they have an
                                                         // active login session on another tab of the same browser
                                                         // Set if you want a specific DocuSign AccountId, If null, the user's default account will be used.
        public string targetAccountId = null;
        public string demoDocPath = "demo_documents";
        public string docDocx = "World_Wide_Corp_Battle_Plan_Trafalgar.docx";
        public string docPdf = "World_Wide_Corp_lorem.pdf";
        public string githubExampleUrl = "https://github.com/docusign/eg-03-csharp-auth-code-grant-core/tree/master/eg-03-csharp-auth-code-grant-core/Controllers/";
        public string documentation = null;
    }
}