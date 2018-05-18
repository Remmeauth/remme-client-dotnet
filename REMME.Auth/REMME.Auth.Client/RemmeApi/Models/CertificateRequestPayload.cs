using Newtonsoft.Json;
using Org.BouncyCastle.Pkcs;
using System;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class CertificateRequestPayload
    {
        public const string CERTIFICATE_REQUEST_FORMAT =
            "-----BEGIN CERTIFICATE REQUEST-----\n{0}\n-----END CERTIFICATE REQUEST-----";

        public CertificateRequestPayload() { }

        public CertificateRequestPayload(Pkcs10CertificationRequest request)
        {
            var csr = Convert.ToBase64String(request.GetEncoded());
            CertificateRequest = string.Format(CERTIFICATE_REQUEST_FORMAT, csr);
        }

        [JsonProperty("certificate")]
        public string CertificateRequest { get; set; }
    }
}
