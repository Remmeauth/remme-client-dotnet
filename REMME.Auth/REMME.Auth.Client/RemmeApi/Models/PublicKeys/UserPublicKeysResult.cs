using Newtonsoft.Json;
using System.Collections.Generic;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class UserPublicKeysResult
    {
        [JsonProperty("pubkey")]
        public string PublicKey { get; set; }

        [JsonProperty("pub_keys")]
        public List<string> PublicKeysAdresses { get; set; }
    }
}
