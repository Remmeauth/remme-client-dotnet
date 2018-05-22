using REMME.Auth.Client.RemmeApi.Models;
using System;

namespace REMME.Auth.Client.Contracts.Models
{
    public interface ITransactionResponse
    {
        string BatchId { get; set; }
        
        event EventHandler<BatchStatus> BatchConfirmed;

        void ConnectToWebSocket();

        void CloseWebSocket();
    }
}
