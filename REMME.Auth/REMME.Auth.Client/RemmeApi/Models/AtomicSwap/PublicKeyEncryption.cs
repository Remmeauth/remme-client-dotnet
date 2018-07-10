using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models.AtomicSwap
{
    public class PublicKeyEncryption
    {
        [JsonProperty("pub_key")]
        public string PublicKey { get; set; }
    }
}
