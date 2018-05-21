﻿using Newtonsoft.Json;
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

        public string NodeAddress { get => _nodeAddress; }

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
                case RemmeMethodsEnum.UserCertificates:
                    methodUrl = "user";
                    break;
            }

            var baseUrl = string.Format("http://{0}/api/v1/{1}", _nodeAddress, methodUrl);
            var output = urlParameter == null ? baseUrl : string.Format("{0}/{1}", baseUrl, urlParameter);

            //TODO: Refactor this code to be more readable
            if (method == RemmeMethodsEnum.UserCertificates) output = output + "/certificates";

            return output;
        }

        #endregion
    }
}
