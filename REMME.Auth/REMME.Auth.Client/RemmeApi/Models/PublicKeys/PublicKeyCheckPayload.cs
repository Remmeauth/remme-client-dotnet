using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class PublicKeyCheckPayload
    {
        [JsonProperty("public_key_address")]
        public string PublicKeyAddress { get; set; }
    }
}
