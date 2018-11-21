using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models.Token
{
    public class BalanceCheckRequest
    {
        [JsonProperty("public_key_address")]
        public string PublicKey { get; set; }
    }
}
