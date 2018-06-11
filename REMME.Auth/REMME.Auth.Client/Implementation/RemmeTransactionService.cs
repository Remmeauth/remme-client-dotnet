using Google.Protobuf;
using REMME.Auth.Client.Contracts;
using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.Crypto;
using REMME.Auth.Client.RemmeApi;
using REMME.Auth.Client.RemmeApi.Models;
using REMME.Auth.Client.RemmeApi.Models.Proto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeTransactionService : IRemmeTransactionService
    {
        private readonly IRemmeAccount _remmeAccount;
        private readonly IRemmeRest _remmeRest;

        public string SignerAddress { get => _remmeAccount.Address; }

        public RemmeTransactionService(IRemmeAccount remmeAccount, IRemmeRest remmeRest)
        {
            _remmeAccount = remmeAccount;
            _remmeRest = remmeRest;
        }

        public async Task<Transaction> CreateTransaction(TransactionCreateDto toCreate)
        {
            var bathcerPublicKey = await GetBatcherPublicKey();
            var header = ComputeHeader(toCreate, bathcerPublicKey);

            return new Transaction
            {
                Header = header.ToByteString(),
                HeaderSignature = _remmeAccount.Sign(header.ToByteArray()),
                Payload = toCreate.Payload
            };
        }

        public async Task<BaseTransactionResponse> SendTransaction(Transaction transaction)
        {
            var transactionPayload = new RawTransactionPayload
                                        {
                                            TransactionBase64 = Convert.ToBase64String(transaction.ToByteArray())
                                        };

            var result = await _remmeRest.PostRequest<RawTransactionPayload, RawTransactionResult>(
                            RemmeMethodsEnum.RawTransaction,
                            transactionPayload);

            return new BaseTransactionResponse(_remmeRest.SocketAddress) { BatchId = result.BachId };
        }

        #region Private Helpers 

        private TransactionHeader ComputeHeader(TransactionCreateDto transactionCreateDto, string bathcerPublicKey)
        {
            var sha512Payload = transactionCreateDto.Payload.ToByteArray().Sha512Digest().BytesToHexString();

            var header = new TransactionHeader
            {
                SignerPublicKey = _remmeAccount.PublicKeyHex,
                BatcherPublicKey = bathcerPublicKey,
                FamilyName = transactionCreateDto.FamilyName,
                FamilyVersion = transactionCreateDto.FamilyVersion,
                Nonce = GetNonce(),
                PayloadSha512 = sha512Payload
            };

            header.Inputs.AddRange(transactionCreateDto.Inputs);
            header.Outputs.AddRange(transactionCreateDto.Outputs);

            return header;
        }

        private string GetNonce()
        {
            return Guid.NewGuid().ToByteArray().BytesToHexString();
        }

        private async Task<string> GetBatcherPublicKey()
        {
            var result = await _remmeRest.GetRequest<NodeKeyResult>(RemmeMethodsEnum.NodePublicKey);        
            return result.NodePublicKey;
        }

        public TransactionCreateDto GenerateTransactionDto(TransactionPayload remmeTransaction, List<string> inputsOutputs, string familyName, string familyVersion)
        {
            return new TransactionCreateDto
            {
                FamilyName = familyName,
                FamilyVersion = familyVersion,
                Inputs = inputsOutputs,
                Outputs = inputsOutputs,
                Payload = remmeTransaction.ToByteString()
            };
        }

        public List<string> GetDataInputOutput(string dataAddress)
        {
            return new List<string> { dataAddress, SignerAddress };
        }

        #endregion
    }
}
