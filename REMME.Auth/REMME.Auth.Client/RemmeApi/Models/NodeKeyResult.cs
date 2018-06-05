using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class NodeKeyResult
    {
        [JsonProperty("pubkey")]
        public string NodePublicKey { get; set; }
    }
}
