using System;
using WebSocketSharp;

namespace REMME.Auth.Client.Contracts.Models
{
    public interface ITransactionResponse
    {
        string BatchId { get; set; }
        
        event EventHandler<MessageEventArgs> BatchConfirmed;

        void ConnectToWebSocket();

        void CloseWebSocket();
    }
}
