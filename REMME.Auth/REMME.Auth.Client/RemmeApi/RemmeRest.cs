using Newtonsoft.Json;
using REMME.Auth.Client.Contracts.Exceptions;
using REMME.Auth.Client.RemmeApi.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace REMME.Auth.Client.RemmeApi
{
    public class RemmeRest: IRemmeRest
    {
        private readonly RemmeNetworkConfig _remmeNetworkConfig;

        public RemmeRest(RemmeNetworkConfig remmeNetworkConfig = null)
        {
            _remmeNetworkConfig = remmeNetworkConfig == null ? new RemmeNetworkConfig() : remmeNetworkConfig;
        }

        public string ApiAddress { get => _remmeNetworkConfig.ApiAddress; }
        public string SocketAddress { get => _remmeNetworkConfig.SocketsAddress; }

        public async Task<Output> GetRequest<Output>(RemmeMethodsEnum method, string requestPayload = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetUrlForRequest(method, requestPayload));

            return await SendRequest<Output>(request);
        }

        public async Task<Output> PutRequest<Input, Output>(RemmeMethodsEnum method, Input requestPayload)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, GetUrlForRequest(method));
            request.Content = GetHttpContent(requestPayload);

            return await SendRequest<Output>(request);
        }

        public async Task<Output> PostRequest<Input, Output>(RemmeMethodsEnum method, Input requestPayload)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, GetUrlForRequest(method));
            request.Content = GetHttpContent(requestPayload);

            return await SendRequest<Output>(request);
        }

        public async Task<Output> DeleteRequest<Input, Output>(RemmeMethodsEnum method, Input requestPayload)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, GetUrlForRequest(method));
            request.Content = GetHttpContent(requestPayload);

            return await SendRequest<Output>(request);
        }

        #region Helpers

        private async Task<Output> SendRequest<Output>(HttpRequestMessage request)
        {
            Output result = default(Output);

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = null;
                try
                {
                    response = await client.SendAsync(request);
                }
                catch (HttpRequestException)
                {
                    throw new RemmeConnectionException("Cannot reach REMME node via REST API");
                }

                var str = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
                    throw new RemmeNodeException(str);

                result = JsonConvert.DeserializeObject<Output>(str);
            }

            return result;
        }

        private StringContent GetHttpContent(object requestPayload)
        {
            var stringPayload = JsonConvert.SerializeObject(requestPayload);
            return new StringContent(stringPayload, Encoding.UTF8, "application/json");
        }

        private string GetUrlForRequest(RemmeMethodsEnum method, string urlParameter = null)
        {
            string methodUrl = string.Empty;
            switch (method)
            {
                case RemmeMethodsEnum.PublicKey:
                    methodUrl = "pub_key";
                    break;
                case RemmeMethodsEnum.Token:
                    methodUrl = "token";
                    break;
                case RemmeMethodsEnum.BatchStatus:
                    methodUrl = "batch_status";
                    break;
                case RemmeMethodsEnum.UserPublicKeys:
                    methodUrl = "user";
                    break;
                case RemmeMethodsEnum.RawTransaction:
                    methodUrl = "transaction";
                    break;
                case RemmeMethodsEnum.NodePublicKey:
                    methodUrl = "node_key";
                    break;
                case RemmeMethodsEnum.AtomicSwapInfo:
                    methodUrl = "atomic-swap";
                    break;
                case RemmeMethodsEnum.AtomicSwapPublicKey:
                    methodUrl = "atomic-swap/pub-key-encryption";
                    break;
            }

            var baseUrl = string.Format("{0}/api/v1/{1}", ApiAddress, methodUrl);
            var output = urlParameter == null ? baseUrl : string.Format("{0}/{1}", baseUrl, urlParameter);

            //TODO: Refactor this code to be more readable
            if (method == RemmeMethodsEnum.UserPublicKeys) output = output + "/pub_keys";

            return output;
        }

        #endregion
    }
}
