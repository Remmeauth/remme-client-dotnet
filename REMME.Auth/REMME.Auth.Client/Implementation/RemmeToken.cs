using REMME.Auth.Client.Contracts;
using System;
using System.Threading.Tasks;
using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.RemmeApi;
using REMME.Auth.Client.RemmeApi.Models.Token;
using System.Text.RegularExpressions;
using REMME.Auth.Client.RemmeApi.Models.Proto;
using System.Collections.Generic;
using Google.Protobuf;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeToken : IRemmeToken
    {
        private const string FAMILY_NAME = "account";
        private const string FAMILY_VERSION = "0.1";

        private readonly IRemmeRest _remmeRest;
        private readonly IRemmeTransactionService _remmeTransactionService;

        public RemmeToken(IRemmeRest remmeRest, IRemmeTransactionService remmeTransactionService)
        {
            _remmeRest = remmeRest;
            _remmeTransactionService = remmeTransactionService;
        }

        public async Task<int> GetBalance(string publicKey)
        {
            ValidatePublicKey(publicKey);

            var result = await _remmeRest.GetRequest<BalanceCheckResult>(RemmeMethodsEnum.Token, publicKey);

            return Convert.ToInt32(result.Balance);
        }

        public async Task<BaseTransactionResponse> Transfer(string publicKeyTo, ulong amount)
        {
            ValidatePublicKey(publicKeyTo);

            var transferProto = GenerateTransferPayload(publicKeyTo, amount);
            var remmeTransaction = GenerateRemmeTransaction(transferProto);
            var transactionDto = GenerateTransactionDto(remmeTransaction,
                                                        GetTransferInputOutput(transferProto.AddressTo));

            var resultTrans = await _remmeTransactionService.CreateTransaction(transactionDto);

            return await _remmeTransactionService.SendTransaction(resultTrans);
        }

        #region Private Helpers

        private TransactionCreateDto GenerateTransactionDto(TransactionPayload remmeTransaction, List<string> inputsOutputs)
        {
            return new TransactionCreateDto
            {
                FamilyName = FAMILY_NAME,
                FamilyVersion = FAMILY_VERSION,
                Inputs = inputsOutputs,
                Outputs = inputsOutputs,
                Payload = remmeTransaction.ToByteString()
            };
        }

        private TransactionPayload GenerateRemmeTransaction(TransferPayload transferPayload)
        {
            return new TransactionPayload
            {
                Method = 0,
                Data = transferPayload.ToByteString()
            };
        }

        private List<string> GetTransferInputOutput(string addressTo)
        {
            return new List<string> { addressTo, _remmeTransactionService.SignerAddress };
        }

        private TransferPayload GenerateTransferPayload(string publicKeyTo, ulong amount)
        {            
            return new TransferPayload
            {
                AddressTo = Utils.GetAddressFromData(publicKeyTo, FAMILY_NAME),
                Value = amount
            };
        }

        private bool IsValidPublicKey(string publicKey)
        {
            var patern = @"^[0-9a-f]{66}$";
            return new Regex(patern, RegexOptions.IgnoreCase).IsMatch(publicKey);
        }

        private void ValidatePublicKey(string publicKey)
        {
            if (!IsValidPublicKey(publicKey))
                throw new ArgumentException("Public key has invalid format");
        }

        #endregion
    }
}
