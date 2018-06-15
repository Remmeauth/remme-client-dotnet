using Newtonsoft.Json;
using REMME.Auth.Client.RemmeApi.Models.Batch;

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
        public BatchStatusResult BatchStatuses { get; set; }
    }
}
