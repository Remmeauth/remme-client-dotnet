using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models.Token
{
    public class TransferResult
    {
        [JsonProperty("batch_id")]
        public string BachId { get; set; }
    }
}
