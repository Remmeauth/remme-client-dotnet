using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace REMME.Auth.Client.RemmeApi
{
    public class RemmeRest
    {
        private readonly string _nodeAddress;
        public RemmeRest(string nodeAddress = "localhost:8080")
        {
            _nodeAddress = nodeAddress;
        }

        public async Task<Output> PutRequest<Input, Output>(Input requestPayload, RemmeMethodsEnum method)
        {
            Output result = default(Output);
            using (var client = new HttpClient())
            {
                var stringPayload = JsonConvert.SerializeObject(requestPayload);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(GetUrlForRequest(method), httpContent);
                var str = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<Output>(str);
            }

            return result;
        }

        public async Task<Output> PostRequest<Input, Output>(Input requestPayload, RemmeMethodsEnum method)
        {
            Output result = default(Output);
            using (var client = new HttpClient())
            {
                var stringPayload = JsonConvert.SerializeObject(requestPayload);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(GetUrlForRequest(method), httpContent);
                var str = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<Output>(str);
            }

            return result;
        }

        private string GetUrlForRequest(RemmeMethodsEnum method)
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

            return string.Format("http://{0}/api/v1/{1}", _nodeAddress, methodUrl);
        }
    }
}
