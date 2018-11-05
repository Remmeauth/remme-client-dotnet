namespace REMME.Auth.Client.RemmeApi.Models
{
    public class RemmeNetworkConfig
    {
        private const string URL_FORMAT = "{0}://{1}:{2}";
        private const string SOCKET_URL_FORMAT = "{0}/ws";

        public RemmeNetworkConfig()
        {
            NodeAddress = "localhost";
            ApiPort = "8080";
            SslMode = false;
        }

        public string NodeAddress { get; set; }

        public string ApiPort { get; set; }

        public bool SslMode { get; set; }

        public string ApiAddress { get => string.Format(URL_FORMAT, GetHttpProtocol(), NodeAddress, ApiPort); }
        public string SocketsAddress { get => string.Format(SOCKET_URL_FORMAT, string.Format(URL_FORMAT, GetWebSocketsProtocol(), NodeAddress, ApiPort)); }

        private string GetHttpProtocol() => SslMode ? "https" : "http";        
        private string GetWebSocketsProtocol() => SslMode ? "wss" : "ws";        
    }
}
