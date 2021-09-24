# C# Launcher Code Examples

### GitHub repo: [code-examples-csharp](./README.md)

This GitHub repo includes code examples for the DocuSign Admin API, Click API, eSignature REST API, Monitor API, and Rooms API. To switch between API code examples, modify the `ExamplesAPI` setting from `ESignature` to `Admin`, `Click`, `Monitor`, or  `Rooms`, in the appsettings.json file. 


## Introduction
This repo is a C# .NET Core MVC application that demonstrates:  

* Authentication with DocuSign via [Authorization Code Grant](https://developers.docusign.com/platform/auth/authcode).
When the token expires, the user is asked to re-authenticate. The refresh token is not used.  

* Authentication with DocuSign via [JSON Web Token (JWT) Grant](https://developers.docusign.com/platform/auth/jwt/).
When the token expires, it updates automatically.  


## eSignature API

For more information about the scopes used for obtaining authorization to use the eSignature API, see [Required scopes](https://developers.docusign.com/docs/esign-rest-api/esign101/auth#required-scopes).  

For a list of code examples that use the eSignature API, select the C# tab under [Examples and languages](https://developers.docusign.com/docs/esign-rest-api/how-to/code-launchers#examples-and-languages) on the DocuSign Developer Center.


## Rooms API 

**Note:** To use the Rooms API, you must also [create your Rooms developer account](https://developers.docusign.com/docs/rooms-api/rooms101/create-account). Examples 4 and 6 require that you have the DocuSign Forms feature enabled in your Rooms for Real Estate account.  
For more information about the scopes used for obtaining authorization to use the Rooms API, see [Required scopes](https://developers.docusign.com/docs/rooms-api/rooms101/auth/).  

For a list of code examples that use the Rooms API, select the C# tab under [Examples and languages](https://developers.docusign.com/docs/rooms-api/how-to/code-launchers#examples-and-languages) on the DocuSign Developer Center.


## Click API  
For more information about the scopes used for obtaining authorization to use the Click API, see [Required scopes](https://developers.docusign.com/docs/click-api/click101/auth/#required-scopes)  

For a list of code examples that use the Click API, select the C# tab under [Examples and languages](https://developers.docusign.com/docs/click-api/how-to/code-launchers#examples-and-languages) on the DocuSign Developer Center.


## Monitor API

**Note:** To use the Monitor API, you must also [enable DocuSign Monitor for your organization](https://developers.docusign.com/docs/monitor-api/how-to/enable-monitor/).  

For information about the scopes used for obtaining authorization to use the Monitor API, see the [scopes section](https://developers.docusign.com/docs/monitor-api/monitor101/auth/). 

For a list of code examples that use the Monitor API, select the C# tab under [Examples and languages](https://developers.docusign.com/docs/monitor-api/how-to/code-launchers#examples-and-languages) on the DocuSign Developer Center.


## Admin API

**Note:** To use the Admin API, you must [create an organization](https://support.docusign.com/en/guides/org-admin-guide-create-org) in your DocuSign developer account. Also, to run the DocuSign CLM code example, [CLM must be enabled for your organization](https://support.docusign.com/en/articles/DocuSign-and-SpringCM).

For information about the scopes used for obtaining authorization to use the Admin API, see the [scopes section](https://developers.docusign.com/docs/admin-api/admin101/auth/).

For a list of code examples that use the Admin API, select the C# tab under [Examples and languages](https://developers.docusign.com/docs/admin-api/how-to/code-launchers/#examples-and-languages) on the DocuSign Developer Center.


## Installation

### Prerequisites
**Note:** If you downloaded this code using [Quickstart](https://developers.docusign.com/docs/esign-rest-api/quickstart/) from the DocuSign Developer Center, skip items 1 and 2 as they were automatically performed for you.  

1. A free [DocuSign developer account](https://go.docusign.com/o/sandbox/); create one if you don't already have one.  
1. A DocuSign app and integration key that is configured to use either [Authorization Code Grant](https://developers.docusign.com/platform/auth/authcode/) or [JWT Grant](https://developers.docusign.com/platform/auth/jwt/) authentication.  

   This [video](https://www.youtube.com/watch?v=eiRI4fe5HgM) demonstrates how to obtain an integration key.  
   
   To use [Authorization Code Grant](https://developers.docusign.com/platform/auth/authcode/), you will need an integration key and a secret key. See [Installation steps](#installation-steps) for details.  

   To use [JWT Grant](https://developers.docusign.com/platform/auth/jwt/), you will need an integration key, an RSA key pair, and the User ID GUID of the impersonated user. See [Installation steps for JWT Grant authentication](#installation-steps-for-jwt-grant-authentication) for details.  

   For both authentication flows:  
   
   If you use this launcher on your own workstation, the integration key must include a redirect URI of https://localhost:44333/ds/callback  

   If you host this launcher on a remote web server, set your redirect URI as   
   
   {base_url}/ds/callback  
   
   where {base_url} is the URL for the web app.  
   
1. [C# .NET Core](https://dotnet.microsoft.com/download/dotnet-core) 3.1 or later.  
1. [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/) with ASP.NET package.


### Installation steps
**Note:** If you downloaded this code using [Quickstart](https://developers.docusign.com/docs/esign-rest-api/quickstart/) from the DocuSign Developer Center, skip step 4 as it was automatically performed for you.

1. Extract the Quickstart ZIP file or download or clone the code-examples-csharp repository.
1. In File Explorer, open your Quickstart folder or your code-examples-powershell folder. 
1. Open the project with Visual Studio by double-clicking the launcher-csharp.sln file.
1. To configure the launcher for [Authorization Code Grant](https://developers.docusign.com/platform/auth/authcode/) authentication, create a copy of the file launcher-csharp/appsettings.example.json and save the copy as launcher-csharp/appsettings.json.
   1. Add your integration key. On the [Apps and Keys](https://admindemo.docusign.com/authenticate?goTo=apiIntegratorKey) page, under **Apps and Integration Keys**, choose the app to use, then select **Actions > Edit**. Under **General Info**, copy the **Integration Key** GUID and save it in appsettings.json as your `ClientId`.
   1. Generate a secret key, if you don’t already have one. Under **Authentication**, select **+ ADD SECRET KEY**. Copy the secret key and save it in appsettings.json as your `ClientSecret`.
   1. Add the launcher’s redirect URI. Under **Additional settings**, select **+ ADD URI**, and set a redirect URI of https://localhost:44333/ds/callback. Select **SAVE**.   
   1. Set a name and email address for the signer. In appsettings.json, save an email address as `SignerEmail` and a name as `SignerName`.  
1. Run the launcher with Visual Studio: Select the green Play **IIS Express** button.


### Installation steps for JWT Grant authentication

**Note:** If you downloaded this code using [Quickstart](https://developers.docusign.com/docs/esign-rest-api/quickstart/) from the DocuSign Developer Center, skip step 4 as it was automatically performed for you.  
Also, in order to select JSON Web Token authentication in the launcher, in launcher-csharp/appsettings.json, change the `quickstart` setting to `"false"`.  

1. Extract the Quickstart ZIP file or download or clone the code-examples-csharp repository.
1. In File Explorer, open your Quickstart folder or your code-examples-powershell folder. 
1. Open the launcher-csharp.sln solution file using Visual Studio.
1. To configure the launcher for [JWT Grant](https://developers.docusign.com/platform/auth/jwt/) authentication, create a copy of the file launcher-csharp/appsettings.example.json and save the copy as launcher-csharp/appsettings.json.
   1. Add your User ID. On the [Apps and Keys](https://admindemo.docusign.com/authenticate?goTo=apiIntegratorKey) page, under **My Account Information**, copy the **User ID** GUID and save it in appsettings.json as your `ImpersonatedUserId`.
   1. Add your integration key. On the [Apps and Keys](https://admindemo.docusign.com/authenticate?goTo=apiIntegratorKey) page, under **Apps and Integration Keys**, choose the app to use, then select **Actions > Edit**. Under **General Info**, copy the **Integration Key** GUID and save it in appsettings.json as your `ClientId`.
   1. Generate an RSA key pair, if you don’t already have one. Under **Authentication**, select **+ GENERATE RSA**. Copy the private key, and save it in a new file launcher-csharp/private.key.   
   1. Add the launcher’s redirect URI. Under **Additional settings**, select **+ ADD URI**, and set a redirect URI of https://localhost:44333/ds/callback. Select **SAVE**.   
   1. Set a name and email address for the signer. In appsettings.json, save an email address as `SignerEmail` and a name as `SignerName`.  
**Note:** Protect your personal information. Please make sure that appsettings.json will not be stored in your source code repository.  
1. Run the launcher with Visual Studio: Select the green **Play IIS** Express button. 
1. On the black navigation bar, select **Login**.
1. From the picklist, select **JWT (JSON Web Token)** > **Authenticate with DocuSign**.
1. When prompted, log in to your DocuSign developer account. If this is your first time using the app, select **ACCEPT** at the consent window. 
1. Select your desired code example.


## Payments code example  
To use the payments code example, create a test payment gateway on the [Payments](https://admindemo.docusign.com/authenticate?goTo=payments) page in your developer account. See [Configure a payment gateway](./PAYMENTS_INSTALLATION.md) for details.  

Once you've created a payment gateway, save the **Gateway Account ID** GUID to appsettings.json.


## License and additional information  

### License  
This repository uses the MIT License. See [LICENSE](./LICENSE) for details.

### Pull Requests
Pull requests are welcomed. Pull requests will only be considered if their content
uses the MIT License.
