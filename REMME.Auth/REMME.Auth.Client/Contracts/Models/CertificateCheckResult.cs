﻿using Newtonsoft.Json;
namespace REMME.Auth.Client.Contracts.Models
{
    public class CertificateCheckResult
    {
        [JsonProperty("owner")]
        public string OwnerId { get; set; }

        [JsonProperty("revoked")]
        public bool IsRevoked { get; set; }

        public bool IsValid => !IsRevoked;
    }
}
