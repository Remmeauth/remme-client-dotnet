using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REMME.Auth.Client.RemmeApi.Models.Token
{
    public class TransferPayload
    {
        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("pub_key_to")]
        public string PublicKeyTo { get; set; }
    }
}
