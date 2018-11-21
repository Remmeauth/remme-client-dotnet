using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models.PublicKeys
{
    public class GetAccountPublicKeysRequest
    {
        [JsonProperty("public_key_address")]
        public string PublicKeyAddress { get; set; }
    }
}
