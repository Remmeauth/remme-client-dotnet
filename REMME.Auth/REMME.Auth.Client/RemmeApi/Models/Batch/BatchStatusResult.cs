using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models.Batch
{
    public class BatchStatusResult
    {
        [JsonProperty("batch_id")]
        public string BatchId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
