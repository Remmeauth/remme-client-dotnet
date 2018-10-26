using Newtonsoft.Json;
namespace REMME.Auth.Client.RemmeApi.Models.JsonRPC
{
    public class RpcResponseDto
    {
        [JsonProperty("jsonrpc")]
        public string JsonRpcVersion { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("result")]
        public object Result { get; set; }

        [JsonProperty("error")]
        public Error Error { get; set; }
    }

    public class Error
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
