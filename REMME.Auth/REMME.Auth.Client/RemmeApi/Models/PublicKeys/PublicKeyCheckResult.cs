using Newtonsoft.Json;
using System;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class PublicKeyCheckResult
    {
        [JsonProperty("owner_public_key")]
        public string OwnerPublicKey { get; set; }

        [JsonProperty("entity_hash")]
        public string EntityHash { get; set; }

        [JsonProperty("entity_hash_signature")]
        public string EntityHashSignature { get; set; }

        [JsonProperty("is_revoked")]
        public bool IsRevoked { get; set; }

        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }

        [JsonProperty("valid_to")]
        public uint ValidToUnixTime { get; set; }

        [JsonProperty("valid_from")]
        public uint ValidFromUnixTime { get; set; }

        public DateTime ValidToUtc { get => GetDateTimeFromUnixTime(ValidToUnixTime); }
        public DateTime ValidFromUtc { get => GetDateTimeFromUnixTime(ValidFromUnixTime); }

        private DateTime GetDateTimeFromUnixTime(uint timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }
}
