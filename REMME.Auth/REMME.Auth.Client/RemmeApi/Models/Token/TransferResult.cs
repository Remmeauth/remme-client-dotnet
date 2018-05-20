using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models.Token
{
    class TransferResult
    {
        [JsonProperty("batch_id")]
        public string BachId { get; set; }
    }
}
