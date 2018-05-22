using Newtonsoft.Json;
using REMME.Auth.Client.RemmeApi.Models;
using System;
using WebSocketSharp;

namespace REMME.Auth.Client.Contracts.Models
{
    public class BaseTransactionResponse : ITransactionResponse
    {
        private WebSocket _webSocket;
        private string _socketAddress;

        public BaseTransactionResponse(string socketAddress)
        {
            _socketAddress = socketAddress;
        }

        public string BatchId { get; set; }

        public event EventHandler<BatchStatus> BatchConfirmed;

        public void CloseWebSocket()
        {
            if (_webSocket != null && _webSocket.IsAlive)
            {
                _webSocket.Send(GetSocketQueryMessage(subscribe: false));
                _webSocket.CloseAsync();
            }
        }

        public void ConnectToWebSocket()
        {
            if (_webSocket != null && _webSocket.IsAlive)
            {
                CloseWebSocket();
            }
            _webSocket = new WebSocket(GetSubscribeUrl());

            if (BatchConfirmed != null)
                _webSocket.OnMessage += _webSocket_OnMessage;

            _webSocket.Connect();
            _webSocket.Send(GetSocketQueryMessage());

        }

        private void _webSocket_OnMessage(object sender, MessageEventArgs e)
        {
            var response = JsonConvert.DeserializeObject<BatchStateUpdateDto>(e.Data);

            if(response.Type == "message" && response.Data != null)
            {
                switch (response.Data.BatchStatuses.Status)
                {
                    case "OK":
                        BatchConfirmed(sender, response.Data.BatchStatuses);
                        break;
                    default:
                        break;
                }
            }
        }

        private string GetSubscribeUrl()
        {
            return string.Format("ws://{0}/ws", _socketAddress);
        }

        private string GetSocketQueryMessage(bool subscribe = true)
        {
            var requestData = new BatchStatusQueryDto()
            {
                Type = "request",
                Action = subscribe ? "subscribe" : "unsubscribe",
                Entity = "batch_state",
                Id = new Random().Next(1, 10000)
            };
            requestData.Parameters.BatchIds.Add(BatchId);
            return JsonConvert.SerializeObject(requestData);
        }
    }
}
