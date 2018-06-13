using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class PublicKeyCheckPayload
    {
        [JsonProperty("pub_key")]
        public string PublicKeyPem { get; set; }
    }
}
