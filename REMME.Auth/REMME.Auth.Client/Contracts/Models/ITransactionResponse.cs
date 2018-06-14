using REMME.Auth.Client.RemmeApi.Models.Batch;
using System;

namespace REMME.Auth.Client.Contracts.Models
{
    public interface ITransactionResponse
    {
        string BatchId { get; set; }
        
        event EventHandler<BatchStatusResult> OnREMChainMessage;

        void ConnectToWebSocket();

        void CloseWebSocket();
    }
}
