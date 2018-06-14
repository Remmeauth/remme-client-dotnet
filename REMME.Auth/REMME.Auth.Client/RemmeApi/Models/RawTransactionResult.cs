using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class RawTransactionResult
    {
        [JsonProperty("batch_id")]
        public string BatchId { get; set; }
    }
}
