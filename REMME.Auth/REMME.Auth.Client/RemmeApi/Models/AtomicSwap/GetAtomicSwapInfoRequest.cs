using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models.AtomicSwap
{
    public class GetAtomicSwapInfoRequest
    {
        [JsonProperty("swap_id")]
        public string SwapId { get; set; }
    }
}
