using Google.Protobuf;
using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.RemmeApi.Models.Proto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REMME.Auth.Client.Contracts
{
    /// <summary>
    /// The Interface which includes methods for creating and submitting raw transactions to REMChain
    /// </summary>
    public interface IRemmeTransactionService
    {
        string SignerAddress { get; }

        Task<Transaction> CreateTransaction(TransactionCreateDto toCreate);

        Task<BaseTransactionResponse> SendTransaction(Transaction transaction);

        TransactionCreateDto GenerateTransactionDto(TransactionPayload remmeTransaction, List<string> inputsOutputs, string familyName, string familyVersion);

        List<string> GetDataInputOutput(string dataAddress);

        TransactionPayload GetTransactionPayload(IMessage payload, uint methodNumber);
    }
}
