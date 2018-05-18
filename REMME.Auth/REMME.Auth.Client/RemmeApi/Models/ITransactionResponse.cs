using System;
using System.Threading.Tasks;
using WebSocketSharp;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public interface ITransactionResponse
    {
        string BatchId { get; set; }
        
        event EventHandler<MessageEventArgs> BatchConfirmed;

        void ConnectToWebSocket();

        void CloseWebSocket();
    }
}
