﻿using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class RawTransactionPayload
    {
        [JsonProperty("data")]
        public string TransactionBase64 { get; set; }
    }
}
