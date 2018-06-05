using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class BatchStateUpdateDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("batch_statuses")]
        public BatchStatus BatchStatuses { get; set; }
    }

    public class BatchStatus
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("batch_id")]
        public string BatchId { get; set; }
    }
}
