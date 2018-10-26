﻿using Newtonsoft.Json;
using REMME.Auth.Client.Contracts.Exceptions;
using REMME.Auth.Client.RemmeApi.Models;
using REMME.Auth.Client.RemmeApi.Models.JsonRPC;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace REMME.Auth.Client.RemmeApi
{
    public class RemmeApi : IRemmeApi
    {
        private const string JSON_RPC_VERSION = "2.0";

        private readonly RemmeNetworkConfig _remmeNetworkConfig;

        public RemmeApi(RemmeNetworkConfig remmeNetworkConfig = null)
        {
            _remmeNetworkConfig = remmeNetworkConfig == null ? new RemmeNetworkConfig() : remmeNetworkConfig;
        }

        public string ApiAddress { get => _remmeNetworkConfig.ApiAddress; }
        public string SocketAddress { get => _remmeNetworkConfig.SocketsAddress; }

        public async Task<Output> SendRequest<Input, Output>(RemmeMethodsEnum method, Input requestPayload)
        {
            Output result = default(Output);
            var request = new HttpRequestMessage(HttpMethod.Post, ApiAddress);
            request.Content = GetHttpContent(method, requestPayload);

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = null;
                try
                {
                    response = await client.SendAsync(request);
                }
                catch (HttpRequestException)
                {
                    throw new RemmeConnectionException("Cannot reach REMME node via API");
                }

                var str = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new RemmeNodeException(str);

                result = ParseResponse<Output>(str);
            }

            return result;
        }

        public async Task<Output> SendRequest<Output>(RemmeMethodsEnum method)
        {
            return await SendRequest<object, Output>(method, null);
        }

        #region Helpers

        private Output ParseResponse<Output>(string httpResponseString)
        {
            Output result = default(Output);

            var rpcResult = JsonConvert.DeserializeObject<RpcResponseDto>(httpResponseString);
            var responseType = typeof(Output);

            if(rpcResult.Error != null)
            {
                throw new RemmeNodeException(rpcResult.Error.Message);
            }

            //TODO: Refactor this bad piece
            if (responseType == typeof(string))
                result = (Output)(object)rpcResult.Result;
            else if (responseType == typeof(ulong))
                result = (Output)(object)Convert.ToUInt64(rpcResult.Result);
            else
            {
                result = JsonConvert.DeserializeObject<Output>(rpcResult.Result.ToString());
            }            

            return result;
        }

        private StringContent GetHttpContent(RemmeMethodsEnum method, object requestPayload = null)
        {
            var rpcRequestDto = new RpcRequestDto
            {
                Id = GetRandomId(),
                JsonRpcVersion = JSON_RPC_VERSION,
                MethodName = RemmeMetodsConventer.GetMethodNameString(method)
            };

            if (requestPayload != null)
            {
                rpcRequestDto.AdditionalParameters = requestPayload;
            }

            var jsonDataString = JsonConvert.SerializeObject(rpcRequestDto);
            return new StringContent(jsonDataString, Encoding.UTF8, "application/json");
        }

        private int GetRandomId()
        {
            return new Random().Next(1, 100);
        }

        #endregion 

        ////private string GetUrlForRequest(RemmeMethodsEnum method, string urlParameter = null)
        ////{
        ////    string methodUrl = string.Empty;
        ////    switch (method)
        ////    {
        ////        case RemmeMethodsEnum.PublicKey:
        ////            methodUrl = "pub_key";
        ////            break;
        ////        case RemmeMethodsEnum.Token:
        ////            methodUrl = "token";
        ////            break;
        ////        case RemmeMethodsEnum.BatchStatus:
        ////            methodUrl = "batch_status";
        ////            break;
        ////        case RemmeMethodsEnum.UserPublicKeys:
        ////            methodUrl = "user";
        ////            break;
        ////        case RemmeMethodsEnum.RawTransaction:
        ////            methodUrl = "transaction";
        ////            break;
        ////        case RemmeMethodsEnum.NodePublicKey:
        ////            methodUrl = "node_key";
        ////            break;
        ////        case RemmeMethodsEnum.AtomicSwapInfo:
        ////            methodUrl = "atomic-swap";
        ////            break;
        ////        case RemmeMethodsEnum.AtomicSwapPublicKey:
        ////            methodUrl = "atomic-swap/pub-key-encryption";
        ////            break;
        ////    }

        //    var baseUrl = string.Format("{0}/api/v1/{1}", ApiAddress, methodUrl);
        //    //var output = urlParameter == null ? baseUrl : string.Format("{0}/{1}", baseUrl, urlParameter);

        //    ////TODO: Refactor this code to be more readable
        //    //if (method == RemmeMethodsEnum.UserPublicKeys) output = output + "/pub_keys";

        //    return ApiAddress;
        //}
    }
}
