using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class NodeConfigResponse
    {
        [JsonProperty("node_public_key")]
        public string NodePublicKey { get; set; }

        [JsonProperty("storage_public_key")]
        public string StoragePublicKey { get; set; }
    }
}