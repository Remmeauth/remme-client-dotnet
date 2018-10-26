using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models.Batch
{
    public class GetBatchStatusRequest
    {
        [JsonProperty("id")]
        public string BatchId { get; set; }
    }
}
