namespace REMME.Auth.Client.RemmeApi.Models
{
    public class RemmeNetworkConfig
    {
        private const string URL_FORMAT = "{0}://{1}:{2}";

        public RemmeNetworkConfig()
        {
            NodeAddress = "localhost";
            ApiPort = "8080";
            SocketsPort = "9080";
            ValidatorPort = "8008";
            SslMode = false;
        }

        public string NodeAddress { get; set; }

        public string ApiPort { get; set; }
        public string SocketsPort { get; set; }
        public string ValidatorPort { get; set; }

        public bool SslMode { get; set; }

        public string ApiAddress { get => string.Format(URL_FORMAT, GetHttpProtocol(), NodeAddress, ApiPort); }
        public string SocketsAddress { get => string.Format(URL_FORMAT, GetWebSocketsProtocol(), NodeAddress, SocketsPort); }
        public string ValidatorAddress { get => string.Format(URL_FORMAT, GetHttpProtocol(), NodeAddress, ValidatorPort); }

        private string GetHttpProtocol()
        {
            return SslMode ? "https" : "http";
        }

        private string GetWebSocketsProtocol()
        {
            return SslMode ? "wss" : "ws";
        }
    }
}
