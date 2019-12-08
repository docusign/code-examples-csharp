# C#: Authorization Code Grant Examples

### Github repo: [eg-03-csharp-auth-code-grant-core](./)
## Introduction
This repo is a C# .NET Core MVC application that demonstrates:

* Authentication with DocuSign via the
[Authorization Code Grant flow](https://developers.docusign.com/esign-rest-api/guides/authentication/oauth2-code-grant).
When the token expires, the user is asked to re-authenticate.
The **refresh token** is not used in this example.

## API version 2.1

1. **Embedded Signing Ceremony.**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg001EmbeddedSigningController.cs)
   This example sends an envelope, and then uses an embedded signing ceremony for the first signer.
   With embedded signing, the DocuSign signing ceremony is initiated from your website.
1. **Send an envelope with a remote (email) signer and cc recipient.**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg002SigningViaEmailController.cs)
   The envelope includes a pdf, Word, and HTML document.
   Anchor text ([AutoPlace](https://support.docusign.com/en/guides/AutoPlace-New-DocuSign-Experience)) is used to position the signing fields in the documents.
1. **List envelopes in the user's account.**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg003ListEnvelopesController.cs)
   The envelopes' current status is included.
1. **Get an envelope's basic information.**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg004EnvelopeInfoController.cs)
   The example lists the basic information about an envelope, including its overall status.
1. **List an envelope's recipients**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg005EnvelopeRecipientsController.cs)
   Includes current recipient status.
1. **List an envelope's documents.**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg006EnvelopeDocsController.cs)
1. **Download an envelope's documents.**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg007EnvelopeGetDocController.cs)
   The example can download individual
   documents, the documents concatenated together, or a zip file of the documents.
1. **Programmatically create a template.**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg008CreateTemplateController.cs)
1. **Send an envelope using a template.**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg009UseTemplateController.cs)
1. **Send an envelope and upload its documents with multpart binary transfer.**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg010SendBinaryDocsController.cs)
   Binary transfer is 33% more efficient than using Base64 encoding.
1. **Embedded sending.**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg011EmbeddedSendingController.cs)
   Embeds the DocuSign web tool (NDSE) in your web app to finalize or update
   the envelope and documents before they are sent.
1. **Embedded DocuSign web tool (NDSE).**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg012EmbeddedConsoleController.cs)
1. **Embedded Signing Ceremony from a template with an added document.**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg013AddDocToTemplateController.cs)
   This example sends an envelope based on a template.
   In addition to the template's document(s), the example adds an
   additional document to the envelope by using the
   [Composite Templates](https://developers.docusign.com/esign-rest-api/guides/features/templates#composite-templates)
   feature.
1. **Payments example: an order form, with online payment by credit card.**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg014CollectPaymentController.cs)
1. **Get the envelope tab data.**
   Retrieve the tab (field) values for all of the envelope's recipients.
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg015EnvelopeTabData.cs)
1. **Set envelope tab values.**
   The example creates an envelope and sets the initial values for its tabs (fields). Some of the tabs
   are set to be read-only, others can be updated by the recipient. The example also stores
   metadata with the envelope.
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg016SetTabValues.cs)
1. **Set template tab values.**
   The example creates an envelope using a template and sets the initial values for its tabs (fields).
   The example also stores metadata with the envelope.
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg017SetTemplateTabValues.cs)
1. **Get the envelope custom field data (metadata).**
   The example retrieves the custom metadata (custom data fields) stored with the envelope.
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg018EnvelopeCustomFieldDataController.cs)
1. **Requiring an Access Code for a Recipient**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg019AccessCodeAuthController.cs)
   This example sends an envelope that requires an access-code for the purpose of multi-factor authentication.   
1. **Requiring SMS authentication for a recipient**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg020SmsAuthController.cs)
   This example sends an envelope that requires entering in a six digit code from an text message for the purpose of multi-factor authentication.   
1. **Requiring Phone authentication for a recipient**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg021PhoneAuthController.cs)
   This example sends an envelope that requires entering in a voice-based response code for the purpose of multi-factor authentication.  
1. **Requiring Knowledge-Based Authentication (KBA) for a Recipient**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg022KbaAuthController.cs)
   This example sends an envelope that requires passing a Public records check to validate identity for the purpose of multi-factor authentication.    
1. **Requiring ID Verification (IDV) for a recipient**
   [Source.](./eg-03-csharp-auth-code-grant-core/Controllers/Eg023IdvAuthController.cs)
   This example sends an envelope that requires submitting a photo of a government issued id for the purpose of multi-factor authentication.    
      


## Installation

### Prerequisites
1. A DocuSign Developer Sandbox account (email and password) on [demo.docusign.net](https://demo.docusign.net).
   Create a [free account](https://go.docusign.com/sandbox/productshot/?elqCampaignId=16531).
1. A DocuSign Integration Key (a client ID) that is configured to use the
   OAuth Authorization Code flow.
   You will need the **Integration Key** itself, and its **secret**.

   If you use this example on your own workstation,
   the Integration key must include a **Redirect URI** of `http://localhost:8080/ds/callback`

   If you will not be running the example on your own workstation,
   use the appropriate DNS name and port instead of `localhost`

1. C# .NET Core version 2.1 or later.
1. A name and email for a signer, and a name and email for a cc recipient.

### Installation steps
* Download or clone this repository.
* The repository includes a Visual Studio 2017 solution file and 
NuGet package references in the project file.
* Configure the project by editing the existing project file for 
the API version you wish to use by modifying its
  `appsettings.json`

  See the Configuration section, below, for more information.

### Configuration
**appsettings.json** is the configuration file.

Replace the {CLIENT_ID}, {CLIENT_SECRET}, {USER_EMAIL}, and {USER_FULLNAME}
text with your values. Do not leave the braces { } in the configuration file.

The Client_id (Integration Key) and its secret are private. Please do not
store the appsettings file in your code repository after you have added
the private information to it.

To update to production, change all of the authorization service
**https://account-d.docusign.com** addresses to **https://account.docusign.com**

#### Payments code example
To use the payments example, create a
test payments gateway for your developer sandbox account.

See the
[PAYMENTS_INSTALLATION.md](./PAYMENTS_INSTALLATION.md)
file for instructions.

Then add the payment gateway account id to the **appsettings.json** file.

### Running the example
Build and then start the solution.

Your default browser will be opened to https://localhost:8080 and you will
see the application's home page.

## Using the examples with other authentication flows

The examples in this repository can also be used with either the
Implicit Grant or JWT OAuth flows.
See the [Authentication guide](https://developers.docusign.com/esign-rest-api/guides/authentication)
for information on choosing the right authentication flow for your application.

## License and additional information

### License
This repository uses the MIT License. Please see the LICENSE file for more information.

### Pull Requests
Pull requests are welcomed. Pull requests will only be considered if their content
uses the MIT License.
