// <copyright file="CreateBrand.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace ESignature.Examples
{
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;

    public static class CreateBrand
    {
        /// <summary>
        /// Creates a brand
        /// </summary>
        /// <param name="brandName">The name of new brand</param>
        /// <param name="defaultBrandLanguage">Default brand's language</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>A brand</returns>
        public static BrandsResponse Create(string brandName, string defaultBrandLanguage, string accessToken, string basePath, string accountId)
        {
            // Construct your API headers
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Construct your request body
            Brand newBrand = new Brand
            {
                BrandName = brandName,
                DefaultBrandLanguage = defaultBrandLanguage,
            };

            // Call the eSignature REST API
            AccountsApi accountsApi = new AccountsApi(docuSignClient);

            return accountsApi.CreateBrand(accountId, newBrand);
        }
    }
}
