using Newtonsoft.Json;
using System.Collections.Generic;

namespace REMME.Auth.Client.RemmeApi.Models.Certificate
{
    public class UserCertificatesResult
    {
        [JsonProperty("pubkey")]
        public string PublicKey { get; set; }

        [JsonProperty("certificates")]
        public List<string> CertificateAdresses { get; set; }
    }
}
