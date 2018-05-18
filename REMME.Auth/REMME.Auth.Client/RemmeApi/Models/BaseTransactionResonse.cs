using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class BaseTransactionResonse : ITransactionResponse
    {
        private WebSocket _webSocket;
        private string _socketAddress;

        public BaseTransactionResonse(string socketAddress)
        {
            _socketAddress = socketAddress;
        }

        public string BatchId { get; set; }

        public event EventHandler<MessageEventArgs> BatchConfirmed;

        public void CloseWebSocket()
        {
            if (_webSocket != null && _webSocket.IsAlive)
                _webSocket.CloseAsync();
        }

        public void ConnectToWebSocket()
        {
            if (_webSocket != null && _webSocket.IsAlive)
            {
                CloseWebSocket();
            }
            _webSocket = new WebSocket(_socketAddress);

            if (BatchConfirmed != null)
                _webSocket.OnMessage += (sender, e) => BatchConfirmed(sender, e);

            _webSocket.Connect();

            //TODO format message in right way after sockets will be implemented
            //First message should be formatted with using BathID
            _webSocket.Send(GetSocketSubscribeMessage());

            //Here should be check of response data and different Events should triggered
            //BathcConfirmed, BatchRejected .... 
        }

        private string GetSocketSubscribeMessage()
        {
            //TODO generate message from BatchID
            return BatchId;
        }
    }
}
