# C#: DocuSign Code Examples and Launcher

This GitHub repo includes code example for both the DocuSign eSignature REST API as well as the DocuSign Rooms API. To use the Rooms API code example, modify the **ExampleAPI** settings at the end of the appsettings.json file from ESignature to Rooms.

**Note:** to use the Rooms API you must also [create your DocuSign Developer Account for Rooms](https://developers.docusign.com/docs/rooms-api/rooms101/create-account). 

### GitHub repo: [code-examples-csharp](https://github.com/docusign/code-examples-csharp)
## Introduction
This repo is a C# .NET Core MVC application that demonstrates:

* Authentication with DocuSign via the
[Authorization Code Grant flow](https://developers.docusign.com/esign-rest-api/guides/authentication/oauth2-code-grant).
When the token expires, the user is asked to re-authenticate.
The **refresh token** is not used in this example.

## eSignature API

1. **Use embedded signing.**
   [Source.](./launcher-csharp/Eg001EmbeddedSigningController.cs)
   This example sends an envelope, and then uses embedded signing for the first signer.
   With embedded signing, DocuSign signing is initiated from your website.
1. **Send an envelope with a remote (email) signer and cc recipient.**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg002SigningViaEmailController.cs)
   The envelope includes a pdf, Word, and HTML document.
   Anchor text ([AutoPlace](https://support.docusign.com/en/guides/AutoPlace-New-DocuSign-Experience)) is used to position the signing fields in the documents.
1. **List envelopes in the user's account.**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg003ListEnvelopesController.cs)
   The envelope's current status is included.
1. **Get an envelope's basic information.**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg004EnvelopeInfoController.cs)
   The example lists the basic information about an envelope, including its overall status.
1. **List an envelope's recipients**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg005EnvelopeRecipientsController.cs)
   Includes current recipient status.
1. **List an envelope's documents.**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg006EnvelopeDocsController.cs)
1. **Download an envelope's documents.**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg007EnvelopeGetDocController.cs)
   The example can download individual
   documents, the documents concatenated together, or a zip file of the documents.
1. **Programmatically create a template.**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg008CreateTemplateController.cs)
1. **Send an envelope using a template.**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg009UseTemplateController.cs)
1. **Send an envelope and upload its documents with multpart binary transfer.**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg010SendBinaryDocsController.cs)
   Binary transfer is 33% more efficient than using Base64 encoding.
1. **Use embedded sending.**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg011EmbeddedSendingController.cs)
   Embeds the DocuSign web tool (NDSE) in your web app to finalize or update
   the envelope and documents before they are sent.
1. **Embedded DocuSign web tool (NDSE).**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg012EmbeddedConsoleController.cs)
1. **Use embedded signing from a template with an added document.**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg013AddDocToTemplateController.cs)
   This example sends an envelope based on a template.
   In addition to the template's document(s), the example adds an
   additional document to the envelope by using the
   [Composite Templates](https://developers.docusign.com/esign-rest-api/guides/features/templates#composite-templates)
   feature.
1. **Payments example: an order form, with online payment by credit card.**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg014CollectPaymentController.cs)
1. **Get the envelope tab data.**
   Retrieve the tab (field) values for all of the envelope's recipients.
   [Source.](./launcher-csharp/eSignature/Controllers/Eg015EnvelopeTabData.cs)
1. **Set envelope tab values.**
   The example creates an envelope and sets the initial values for its tabs (fields). Some of the tabs
   are set to be read-only, others can be updated by the recipient. The example also stores
   metadata with the envelope.
   [Source.](./launcher-csharp/eSignature/Controllers/Eg016SetTabValues.cs)
1. **Set template tab values.**
   The example creates an envelope using a template and sets the initial values for its tabs (fields).
   The example also stores metadata with the envelope.
   [Source.](./launcher-csharp/eSignature/Controllers/Eg017SetTemplateTabValues.cs)
1. **Get the envelope custom field data (metadata).**
   The example retrieves the custom metadata (custom data fields) stored with the envelope.
   [Source.](./launcher-csharp/eSignature/Controllers/Eg018EnvelopeCustomFieldDataController.cs)
1. **Requiring an Access Code for a Recipient**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg019AccessCodeAuthController.cs)
   This example sends an envelope that requires an access-code for the purpose of multi-factor authentication.   
1. **Requiring SMS authentication for a recipient**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg020SmsAuthController.cs)
   This example sends an envelope that requires entering in a six digit code from an text message for the purpose of multi-factor authentication.   
1. **Requiring Phone authentication for a recipient**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg021PhoneAuthController.cs)
   This example sends an envelope that requires entering in a voice-based response code for the purpose of multi-factor authentication.  
1. **Requiring Knowledge-Based Authentication (KBA) for a Recipient**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg022KbaAuthController.cs)
   This example sends an envelope that requires passing a Public records check to validate identity for the purpose of multi-factor authentication.    
1. **Requiring ID Verification (IDV) for a recipient**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg023IdvAuthController.cs)
   This example sends an envelope that requires submitting a photo of a government issued id for the purpose of multi-factor authentication.    
1. **Creating a permission profile**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg024PermissionCreateController.cs)
   This code example demonstrates how to create a permission profile using the [Create Permission Profile](https://developers.docusign.com/esign-rest-api/reference/Accounts/AccountPermissionProfiles/create) method.
1. **Setting a permission profile**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg025PermissionSetUserGroupController.cs)
   This code example demonstrates how to set a user group's permission profile using the [Update Group](https://developers.docusign.com/esign-rest-api/reference/UserGroups/Groups/update) method. 
   You must have already created permissions profile and group of users.
1. **Updating individual permission settings**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg026PermissionChangeSingleSettingController.cs)
   This code example demonstrates how to edit individual permission settings on a permissions profile using the [Update Permission Profile](https://developers.docusign.com/esign-rest-api/reference/Accounts/AccountPermissionProfiles/update) method.
1. **Deleting a permission profile**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg027PermissionDeleteController.cs)
   This code example demonstrates how to delete a permission profile using the [Delete Permission Profile](https://developers.docusign.com/esign-rest-api/reference/Accounts/AccountPermissionProfiles/create) method.
1. **Creating a brand**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg028CreateBrandController.cs)
   This example creates a brand profile for an account using the [Create Brand](https://developers.docusign.com/esign-rest-api/reference/Accounts/AccountBrands/create) method.
1. **Applying a brand to an envelope**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg029ApplyBrandToEnvelopeController.cs)
   This code example demonstrates how to apply a brand you've created to an envelope using the [Create Envelope](https://developers.docusign.com/esign-rest-api/reference/Envelopes/Envelopes/create) method. 
   First, the code creates the envelope and then applies the brand to it.
   Anchor text ([AutoPlace](https://support.docusign.com/en/guides/AutoPlace-New-DocuSign-Experience)) is used to position the signing fields in the documents.
1. **Applying a brand to a template**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg030ApplyBrandToTemplateController.cs)
   This code example demonstrates how to apply a brand you've created to a template using using the [Create Envelope](https://developers.docusign.com/esign-rest-api/reference/Envelopes/Envelopes/create) method. 
   You must have at least one created template and brand.
   Anchor text ([AutoPlace](https://support.docusign.com/en/guides/AutoPlace-New-DocuSign-Experience)) is used to position the signing fields in the documents.
1. **Bulk sending envelopes to multiple recipients**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg031BulkSendEnvelopes.cs)
   This code example demonstrates how to send envelopes in bulk to multiple recipients using these methods:
   [Create Bulk Send List](https://developers.docusign.com/esign-rest-api/reference/BulkEnvelopes/BulkSend/createBulkSendList), 
   [Create Bulk Send Request](https://developers.docusign.com/esign-rest-api/reference/BulkEnvelopes/BulkSend/createBulkSendRequest).
   First, the code creates a bulk send recipients list, and then creates an envelope.
   After that, initiates bulk envelope sending.
1. **Pause a signature workflow**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg032PauseSignatureWorkflowController.cs)
   This code example demonstrates how to create an envelope where the workflow is paused before the envelope is sent to a second recipient using the [Create Envelope](https://developers.docusign.com/esign-rest-api/reference/Envelopes/Envelopes/create) method.
1. **Unpause a signature workflow**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg033UnpauseSignatureWorkflowController.cs)
   This code example demonstrates how to update an envelope to resume the workflow that has been paused using the [Update Envelope](https://developers.docusign.com/esign-rest-api/reference/Envelopes/Envelopes/update) method.
   You must have created at least one envelope with a paused signature workflow.
1. **Conditional recipients**
   [Source.](./launcher-csharp/eSignature/Controllers/Eg034ConditionalRecipientsWorkflowController.cs)
   This code example demonstrates how to create an envelope where the workflow is routed to different recipients based on the value of a transaction using the [Create Envelope](https://developers.docusign.com/esign-rest-api/reference/Envelopes/Envelopes/create) method.

## Rooms API 
**Note:** to use the Rooms API you must also [create your DocuSign Developer Account for Rooms](https://developers.docusign.com/docs/rooms-api/rooms101/create-account). 


1. **Create room with Data.**
   [Source.](./launcher-csharp/Rooms/Controllers/Eg01CreateRoomWithDataController.cs)
   This example creates a new room in your DocuSign Rooms account to be used for a transaction.
1. **Create a room from a template.**
   [Source.](./launcher-csharp/Rooms/Controllers/Eg02CreateRoomFromTemplateController.cs)
   This example creates a new room using a template.
1. **Create room with Data.**
   [Source.](./launcher-csharp/Rooms/Controllers/Eg03ExportDataFromRoomController.cs)
   This example exports all the avialalble data from a specific room in your DocuSign Rooms account.
1. **Add forms to a room.**
   [Source.](./launcher-csharp/Rooms/Controllers/Eg04AddingFormToRoomController.cs)
   This example adds a standard real estate related form to a specific room in your DocuSign Rooms account.
1. **How to search for rooms with filters.**
   [Source.](./launcher-csharp/Rooms/Controllers/Eg05GetRoomsWithFiltersController.cs)
   This example searches for rooms in your DocuSign Rooms account using a specific filter. 
1. **Create an external form fillable session.**
   [Source.](./launcher-csharp/Rooms/Controllers/Eg06CreateExternalFormFillSessionController.cs)
   This example create an external form that can be filled using DocuSign for a specific room in your DocuSign Rooms account.


## Installation

### Prerequisites
**Note: If you downloaded this code using Quickstart from the DocuSign Developer Center, skip steps 1 and 2 below as they're automatically performed for you.**

1. A DocuSign developer account (email and password) on [demo.docusign.net](https://demo.docusign.net). Create a [free account](https://go.docusign.com/sandbox/productshot/?elqCampaignId=16531).
1. A DocuSign Integration Key (a client ID) that is configured to use the OAuth Authorization Code flow.
   You will need the **Integration Key** itself, and its **secret**.

   If you use this example on your own workstation, the Integration key must include a **Redirect URI** of `https://localhost:44333/ds/callback`

   If you will not be running the example on your own workstation, use the appropriate DNS name and port instead of `localhost`
   
   This [video](https://www.youtube.com/watch?v=eiRI4fe5HgM) demonstrates how to create an Integration Key (client id) for a user application like this example.

1. [C# .NET Core](https://dotnet.microsoft.com/download/dotnet-core) version 3.1 or later.
1. [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/) with ASP.NET package.
1. A name and email for a signer and a name and email for a cc recipient.

### Installation steps
* Download or clone this repository.
* The repository includes a Visual Studio 2019 solution file and NuGet package references in the project file.
* Configure the project by editing the existing project file for the API version you wish to use by modifying its `appsettings.json`

  See the Configuration section, below, for more information.

### Configuration
**Note: If you downloaded this code using Quickstart from the DocuSign Developer Center, skip steps 1 and 2 below as they're automatically performed for you.**
1. Create the **appsettings.json** (the configuration file) by using the example provided in **appsettings.example.json**.
2. Two different authentication methods are supported:

#### Authorization Code Grant
1. Update the values of **ClientId**, and **ClientSecret** text with your values. 

#### For JWT (JSON Web Tokens)
1. Replace the **ClientId**, and **ImpersonatedUserId** text with your values. 
2. Add and **RSA Private Key** from **DocuSign eSignature Admin** and copy/paste its value into private.key

See the [Authentication guide](https://developers.docusign.com/esign-rest-api/guides/authentication)
for information on choosing the right authentication flow for your application.

The Client_id (Integration Key) and its secret are private. Please do not
store the appsettings file in your code repository after you have added
the private information to it.

To update to production, change all of the authorization service
**https://account-d.docusign.com** addresses to **https://account.docusign.com**

#### Payments code example
To use the payments example, create a
test payments gateway for your developer account.

See the
[PAYMENTS_INSTALLATION.md](./PAYMENTS_INSTALLATION.md)
file for instructions.

Then add the payment gateway account id to the **appsettings.json** file.

### Running the example
Build and then start the solution.

Your default browser will be opened to https://localhost:44333 and you will
see the application's home page.


## License and additional information

### License
This repository uses the MIT License. Please see the LICENSE file for more information.

### Pull Requests
Pull requests are welcomed. Pull requests will only be considered if their content
uses the MIT License.

### Additional Resources
* [DocuSign Developer Center](https://developers.docusign.com)
* [DocuSign API on Twitter](https://twitter.com/docusignapi)
* [DocuSign For Developers on LinkedIn](https://www.linkedin.com/showcase/docusign-for-developers/)
* [DocuSign For Developers on YouTube](https://www.youtube.com/channel/UCJSJ2kMs_qeQotmw4-lX2NQ)
