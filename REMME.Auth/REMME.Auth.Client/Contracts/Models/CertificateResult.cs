﻿using Newtonsoft.Json;

namespace REMME.Auth.Client.Contracts.Models
{
    public class CertificateResult
    {
        [JsonProperty("batch_id")]
        public string BachId { get; set; }

        [JsonProperty("certificate")]
        public string CertificatePEM { get; set; }
    }
}
