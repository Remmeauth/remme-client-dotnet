using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class CertificatePayload
    {
        public const string CERTIFICATE_FORMAT =
    "-----BEGIN CERTIFICATE-----\n{0}\n-----END CERTIFICATE-----";

        public CertificatePayload() { }

        public CertificatePayload(X509Certificate2 certificate)
        {
            var bytes = certificate.Export(X509ContentType.Cert);

            var csr = Convert.ToBase64String(bytes);
            Certificate = string.Format(CERTIFICATE_FORMAT, csr);
        }

        [JsonProperty("certificate")]
        public string Certificate { get; set; }
    }
}
