using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models.Token
{
    public class BalanceCheckResult
    {
        [JsonProperty("balance")]
        public string Balance { get; set; }

        [JsonProperty("pub_key")]
        public bool PublicKey { get; set; }
    }
}
