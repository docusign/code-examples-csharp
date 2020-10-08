using System.Collections.Generic;
using System.Linq;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.CodeExamples.Controllers
{
    [Area("eSignature")]
    [Route("eg007")]
    public class Eg007EnvelopeGetDocController : EgController
    {
        public Eg007EnvelopeGetDocController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Download a document";
        }

        public override string EgName => "eg007";

        // ***DS.snippet.0.start
        private FileStreamResult DoWork(string accessToken, string basePath, string accountId,
            string envelopeId, List<EnvelopeDocItem> documents, string docSelect)
        {
            // Data for this method
            // accessToken
            // basePath
            // accountId
            // envelopeId
            // docSelect -- the requested documentId 
            // documents -- from eg 6
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var envelopesApi = new EnvelopesApi(apiClient);

            // Step 1. EnvelopeDocuments::get.
            // Exceptions will be caught by the calling function
            System.IO.Stream results = envelopesApi.GetDocument(accountId,
                            envelopeId, docSelect);

            // Step 2. Look up the document from the list of documents 
            EnvelopeDocItem docItem = documents.FirstOrDefault(d => docSelect.Equals(d.DocumentId));
            // Process results. Determine the file name and mimetype
            string docName = docItem.Name;
            bool hasPDFsuffix = docName.ToUpper().EndsWith(".PDF");
            bool pdfFile = hasPDFsuffix;
            // Add .pdf if it's a content or summary doc and doesn't already end in .pdf
            string docType = docItem.Type;
            if (("content".Equals(docType) || "summary".Equals(docType)) && !hasPDFsuffix)
            {
                docName += ".pdf";
                pdfFile = true;
            }
            // Add .zip as appropriate
            if ("zip".Equals(docType))
            {
                docName += ".zip";
            }
            // Return the file information
            // See https://stackoverflow.com/a/30625085/64904
            string mimetype;
            if (pdfFile)
            {
                mimetype = "application/pdf";
            }
            else if ("zip".Equals(docType))
            {
                mimetype = "application/zip";
            }
            else
            {
                mimetype = "application/octet-stream";
            }

            return File(results, mimetype, docName);
        }
        // ***DS.snippet.0.end



        [HttpPost]
        public ActionResult Create(string docSelect)
        {
            // Data for this method
            // docSelect -- argument
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;
            var envelopeId = RequestItemsService.EnvelopeId;
            // documents data for the envelope. See example EG006
            List<EnvelopeDocItem> documents = RequestItemsService.EnvelopeDocuments.Documents;

            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation 
                // so it could be restarted automatically.
                // But since it should be rare to have a token issue here,
                // we'll make the user re-enter the form data after 
                // authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            FileStreamResult result = DoWork(accessToken, basePath, accountId,
                envelopeId, documents, docSelect);
            return result;
        }
    }
}