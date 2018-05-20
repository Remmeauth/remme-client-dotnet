using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace REMME.Auth.Client.RemmeApi
{
    public class RemmeRest
    {
        private readonly string _nodeAddress;
        public RemmeRest(string nodeAddress = "localhost:8080")
        {
            _nodeAddress = nodeAddress;
        }

        public string Address { get => _nodeAddress; }

        public async Task<Output> GetRequest<Output>(RemmeMethodsEnum method, string requestPayload = null)
        {
            Output result = default(Output);
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(GetUrlForRequest(method, requestPayload));
                var str = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<Output>(str);
            }

            return result;
        }

        public async Task<Output> PutRequest<Input, Output>(RemmeMethodsEnum method, Input requestPayload)
        {
            Output result = default(Output);
            using (var client = new HttpClient())
            {
                var response = await client.PutAsync(GetUrlForRequest(method), GetHttpContent(requestPayload));
                var str = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<Output>(str);
            }

            return result;
        }

        public async Task<Output> PostRequest<Input, Output>(RemmeMethodsEnum method, Input requestPayload)
        {
            Output result = default(Output);
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(GetUrlForRequest(method), GetHttpContent(requestPayload));
                var str = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<Output>(str);
            }

            return result;
        }

        public async Task<Output> DeleteRequest<Input, Output>(RemmeMethodsEnum method, Input requestPayload)
        {
            Output result = default(Output);
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, GetUrlForRequest(method));
                request.Content = GetHttpContent(requestPayload);

                var response = await client.SendAsync(request);

                var str = await response.Content.ReadAsStringAsync();
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
                case RemmeMethodsEnum.Certificate:
                    methodUrl = "certificate";
                    break;
                case RemmeMethodsEnum.CertificateStore:
                    methodUrl = "certificate/store";
                    break;
                case RemmeMethodsEnum.Token:
                    methodUrl = "token";
                    break;
                case RemmeMethodsEnum.BatchStatus:
                    methodUrl = "batch_status";
                    break;
                case RemmeMethodsEnum.Personal:
                    methodUrl = "personal";
                    break;
            }

            var baseUrl = string.Format("http://{0}/api/v1/{1}", _nodeAddress, methodUrl);
            return urlParameter == null ? baseUrl : string.Format("{0}/{1}", baseUrl, urlParameter);
        }
    }
}
