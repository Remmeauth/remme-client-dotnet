﻿using Newtonsoft.Json;
using REMME.Auth.Client.Contracts.Exceptions;
using REMME.Auth.Client.RemmeApi.Models;
using REMME.Auth.Client.RemmeApi.Models.Batch;
using System;
using WebSocketSharp;

namespace REMME.Auth.Client.Contracts.Models
{
    public class BaseTransactionResponse : ITransactionResponse
    {
        private WebSocket _webSocket;

        public BaseTransactionResponse(string socketAddress)
        {
            SocketAddress = socketAddress;
        }

        public string SocketAddress { get; set; }
        public string BatchId { get; set; }

        public event EventHandler<BatchStatusResult> OnREMChainMessage;

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

            if (OnREMChainMessage != null)
                _webSocket.OnMessage += _webSocket_OnMessage;

            _webSocket.Connect();

            if (_webSocket.ReadyState != WebSocketState.Open)
                throw new RemmeConnectionException("Cannot reach REMME node via sockets endpoint");

            _webSocket.Send(GetSocketQueryMessage());
        }

        private void _webSocket_OnMessage(object sender, MessageEventArgs e)
        {
            var response = JsonConvert.DeserializeObject<BatchStateUpdateDto>(e.Data);

            if (response.Type == "message" && response.Data != null)
            {
                OnREMChainMessage(sender, response.Data.BatchStatuses);
            }
        }

        private string GetSubscribeUrl()
        {
            return SocketAddress;
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
