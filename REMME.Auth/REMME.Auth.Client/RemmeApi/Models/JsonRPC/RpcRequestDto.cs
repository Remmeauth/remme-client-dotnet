using Newtonsoft.Json;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class RpcRequestDto
    {
        [JsonProperty("jsonrpc")]
        public string JsonRpcVersion { get; set; }

        [JsonProperty("method")]
        public string MethodName { get; set; }

        [JsonProperty("params")]
        public object AdditionalParameters { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
