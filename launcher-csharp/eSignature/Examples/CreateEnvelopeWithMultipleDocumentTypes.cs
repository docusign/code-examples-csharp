using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace eSignature.Examples
{
    public static class CreateEnvelopeWithMultipleDocumentTypes
    {
        /// <summary>
        /// Creates an envelope with multiple types of documents and sends it for signature
        /// </summary>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="ccEmail">Email address for the cc recipient</param>
        /// <param name="ccName">Name of the cc recipient</param>
        /// <param name="docPdf">String of bytes representing the document (pdf)</param>
        /// <param name="docDocx">String of bytes representing the Word document (docx)</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>EnvelopeId and any API error information</returns>
        public static (bool statusOk, string envelopeId, string errorCode, string errorMessage, WebException webEx) CreateAndSendEnvelope(
            string signerEmail, string signerName, string ccEmail, string ccName, string docDocx, string docPdf, string accessToken, string basePath, string accountId)
        {
            // Step 1. Make the envelope JSON request body
            dynamic envelope = MakeEnvelope(signerEmail, signerName, ccEmail, ccName);

            // Step 2. Gather documents and their headeres
            // Read files from a local directory
            // The reads could raise an exception if the file is not available! 
            dynamic doc1 = envelope["documents"][0];
            dynamic doc2 = envelope["documents"][1];
            dynamic doc3 = envelope["documents"][2];

            dynamic documents = new[] {
                new {
                    mime = "text/html",
                    filename = (string) doc1["name"],
                    documentId = (string) doc1["documentId"],
                    bytes = Encoding.ASCII.GetBytes(document1(signerEmail, signerName, ccEmail, ccName))
                },
                new {
                    mime = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    filename = (string) doc2["name"],
                    documentId = (string) doc2["documentId"],
                    bytes = System.IO.File.ReadAllBytes(docDocx)
                },
                new {
                    mime = "application/pdf",
                    filename = (string) doc3["name"],
                    documentId = (string) doc3["documentId"],
                    bytes = System.IO.File.ReadAllBytes(docPdf)
                }
            };

            // Step 3. Create the multipart body
            byte[] CRLF = Encoding.ASCII.GetBytes("\r\n");
            byte[] boundary = Encoding.ASCII.GetBytes("multipartboundary_multipartboundary");
            byte[] hyphens = Encoding.ASCII.GetBytes("--");

            string uri = basePath
                    + "/v2.1/accounts/" + accountId + "/envelopes";
            HttpWebRequest request = WebRequest.CreateHttp(uri);

            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "multipart/form-data; boundary=" + Encoding.ASCII.GetString(boundary);
            request.Headers.Add("Authorization", "Bearer " + accessToken);

            using (var buffer = new BinaryWriter(request.GetRequestStream(), Encoding.ASCII))
            {
                buffer.Write(hyphens);
                buffer.Write(boundary);
                buffer.Write(CRLF);
                buffer.Write(Encoding.ASCII.GetBytes("Content-Type: application/json"));
                buffer.Write(CRLF);
                buffer.Write(Encoding.ASCII.GetBytes("Content-Disposition: form-data"));
                buffer.Write(CRLF);
                buffer.Write(CRLF);

                var json = JsonConvert.SerializeObject(envelope, Formatting.Indented);
                buffer.Write(Encoding.ASCII.GetBytes(json));
                // Loop to add the documents.
                // See section Multipart Form Requests on page https://developers.docusign.com/esign-rest-api/guides/requests-and-responses
                foreach (var d in documents)
                {
                    buffer.Write(CRLF);
                    buffer.Write(hyphens);
                    buffer.Write(boundary);
                    buffer.Write(CRLF);
                    buffer.Write(Encoding.ASCII.GetBytes("Content-Type:" + d.mime));
                    buffer.Write(CRLF);
                    buffer.Write(Encoding.ASCII.GetBytes("Content-Disposition: file; filename=\"" + d.filename + ";documentid=" + d.documentId));
                    buffer.Write(CRLF);
                    buffer.Write(CRLF);
                    buffer.Write(d.bytes);
                }

                // Add closing boundary
                buffer.Write(CRLF);
                buffer.Write(hyphens);
                buffer.Write(boundary);
                buffer.Write(hyphens);
                buffer.Write(CRLF);
                buffer.Flush();
            }

            WebResponse response = null;
            WebException webEx = null;
            try
            {
                response = request.GetResponse();
            }
            catch (WebException ex)
            {
                response = ex.Response;
                webEx = ex;
            }

            var res = "";

            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    res = reader.ReadToEnd();
                }
            }

            HttpStatusCode code = ((HttpWebResponse)response).StatusCode;
            dynamic obj = JsonConvert.DeserializeObject(res);
            bool statusOk = code >= HttpStatusCode.OK && code < HttpStatusCode.MultipleChoices;
            string envelopeId = null;
            string errorCode = null;
            string errorMessage = null;

            if (statusOk)
            {
                envelopeId = obj.envelopeId;
            }
            else
            {
                errorCode = obj.errorCode;
                errorMessage = obj.message;
            }

            return (statusOk, envelopeId, errorCode, errorMessage, webEx);
        }

        private static string document1(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName


            return " <!DOCTYPE html>\n" +
                    "    <html>\n" +
                    "        <head>\n" +
                    "          <meta charset=\"UTF-8\">\n" +
                    "        </head>\n" +
                    "        <body style=\"font-family:sans-serif;margin-left:2em;\">\n" +
                    "        <h1 style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\n" +
                    "            color: darkblue;margin-bottom: 0;\">World Wide Corp</h1>\n" +
                    "        <h2 style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\n" +
                    "          margin-top: 0px;margin-bottom: 3.5em;font-size: 1em;\n" +
                    "          color: darkblue;\">Order Processing Division</h2>\n" +
                    "        <h4>Ordered by " + signerName + "</h4>\n" +
                    "        <p style=\"margin-top:0em; margin-bottom:0em;\">Email: " + signerEmail + "</p>\n" +
                    "        <p style=\"margin-top:0em; margin-bottom:0em;\">Copy to: " + ccName + ", " + ccEmail + "</p>\n" +
                    "        <p style=\"margin-top:3em;\">\n" +
                    "  Candy bonbon pastry jujubes lollipop wafer biscuit biscuit. Topping brownie sesame snaps sweet roll pie. Croissant danish biscuit soufflé caramels jujubes jelly. Dragée danish caramels lemon drops dragée. Gummi bears cupcake biscuit tiramisu sugar plum pastry. Dragée gummies applicake pudding liquorice. Donut jujubes oat cake jelly-o. Dessert bear claw chocolate cake gummies lollipop sugar plum ice cream gummies cheesecake.\n" +
                    "        </p>\n" +
                    "        <!-- Note the anchor tag for the signature field is in white. -->\n" +
                    "        <h3 style=\"margin-top:3em;\">Agreed: <span style=\"color:white;\">**signature_1**/</span></h3>\n" +
                    "        </body>\n" +
                    "    </html>";
        }

        private static Dictionary<string, dynamic> MakeEnvelope(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName


            // document 1 (html) has tag **signature_1**
            // document 2 (docx) has tag /sn1/
            // document 3 (pdf) has tag /sn1/
            //
            // The envelope has two recipients.
            // recipient 1 - signer
            // recipient 2 - cc
            // The envelope will be sent first to the signer.
            // After it is signed, a copy is sent to the cc person.
            // create the envelope definition
            // add the documents

            Dictionary<string, dynamic> doc1 = new Dictionary<string, dynamic>()
            {
                { "name", "Order acknowledgement"}, // can be different from actual file name
                { "fileExtension", "html"}, // Source data format. Signed docs are always pdf.
                { "documentId", "1"} // a label used to reference the doc
            };
            Dictionary<string, dynamic> doc2 = new Dictionary<string, dynamic>()
            {
                { "name", "Battle Plan"}, // can be different from actual file name
                { "fileExtension", "docx" },
                { "documentId", "2" }
            };
            Dictionary<string, dynamic> doc3 = new Dictionary<string, dynamic>()
            {
                { "name", "Lorem Ipsum" }, // can be different from actual file name
                { "fileExtension", "pdf" },
                { "documentId", "3" }
            };

            // create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            Dictionary<string, dynamic> signer1 = new Dictionary<string, dynamic>()
            {
                { "email", signerEmail },
                { "name", signerName },
                { "recipientId", "1" },
                { "routingOrder", "1" }
            };
            // routingOrder (lower means earlier) determines the order of deliveries
            // to the recipients. Parallel routing order is supported by using the
            // same integer as the order for two or more recipients.

            // create a cc recipient to receive a copy of the documents, identified by name and email
            // We're setting the parameters via setters
            Dictionary<string, dynamic> cc1 = new Dictionary<string, dynamic>()
            {
                { "email", ccEmail },
                { "name", ccName },
                { "routingOrder", "2" },
                { "recipientId", "2" }
            };
            // Create signHere fields (also known as tabs) on the documents,
            // We're using anchor (autoPlace) positioning
            //
            // The DocuSign platform searches throughout your envelope's
            // documents for matching anchor strings. So the
            // signHere2 tab will be used in both document 2 and 3 since they
            // use the same anchor string for their "signer 1" tabs.
            Dictionary<string, dynamic> signHere1 = new Dictionary<string, dynamic>()
            {
                { "anchorString", "**signature_1**" },
                { "anchorYOffset", "10" },
                { "anchorUnits", "pixels" },
                { "anchorXOffset", "20" }
            };
            Dictionary<string, dynamic> signHere2 = new Dictionary<string, dynamic>()
            {
                { "anchorString", "/sn1/" },
                { "anchorYOffset", "10" },
                { "anchorUnits", "pixels" },
                { "anchorXOffset", "20" }
            };
            // Tabs are set per recipient / signer
            Dictionary<string, dynamic> signer1Tabs = new Dictionary<string, dynamic>()
            {
                { "signHereTabs", new dynamic[] { signHere1, signHere2 } }
            };
            signer1.Add("tabs", signer1Tabs);

            // Recipients holds the different recipient objects as sets of arrays
            Dictionary<string, dynamic> recipients = new Dictionary<string, dynamic>()
            {
                { "signers", new dynamic[] { signer1 } },
                { "carbonCopies", new dynamic[] { cc1 } }
            };

            // create the envelope definition
            Dictionary<string, dynamic> envelopeDefinition = new Dictionary<string, dynamic>()
            {
                { "emailSubject", "Please sign this document set"},
                { "documents", new dynamic[] { doc1, doc2, doc3}},
                { "recipients", recipients },
                { "status", "sent" }
            };

            return envelopeDefinition;
        }
    }
}
