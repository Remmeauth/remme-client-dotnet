using Newtonsoft.Json;
using System.Collections.Generic;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class BatchStatusQueryDto
    {
        public BatchStatusQueryDto()
        {
            Parameters = new Parameters();
        }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("entity")]
        public string Entity { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("parameters")]
        public Parameters Parameters { get; set; }
    }

    public class Parameters
    {
        [JsonProperty("batch_ids")]
        public List<string> BatchIds { get; set; } = new List<string>();
    }
}
