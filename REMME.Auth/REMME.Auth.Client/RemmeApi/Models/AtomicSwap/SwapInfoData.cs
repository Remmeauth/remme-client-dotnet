using Newtonsoft.Json;
using System;

namespace REMME.Auth.Client.RemmeApi.Models.AtomicSwap
{
    public class SwapInfoDto
    {
        [JsonProperty("is_closed")]
        public bool IsClosed { get; set; }

        [JsonProperty("is_approved")]
        public bool IsApproved { get; set; }

        [JsonProperty("receiver_address")]
        public string ReceiverAddress { get; set; }

        [JsonProperty("amount")]
        public uint Amount { get; set; }

        [JsonProperty("email_address_encrypted_optional")]
        public string Email { get; set; }

        [JsonProperty("secret_lock")]
        public string SecretLock { get; set; }

        [JsonProperty("secret_key")]
        public string SecretKey { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAtUtcString { get; set; }

        public DateTime CreatedAtUtc { get => GetDateTimeFromUnixTime(CreatedAtUtcString); }

        [JsonProperty("is_initiator")]
        public bool IsInitiator { get; set; }

        [JsonProperty("sender_address")]
        public string SenderAddress { get; set; }

        [JsonProperty("sender_address_non_local")]
        public string SenderAddressNonLocal { get; set; }

        [JsonProperty("swap_id")]
        public string SwapId { get; set; }

        private DateTime GetDateTimeFromUnixTime(string timeStampString)
        {
            var timestamp = uint.Parse(timeStampString);
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }
}
