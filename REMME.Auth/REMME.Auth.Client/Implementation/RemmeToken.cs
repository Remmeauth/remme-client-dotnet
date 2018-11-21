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
using REMME.Auth.Client.Implementation.Utils;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeToken : IRemmeToken
    {
        private const string FAMILY_NAME = "account";
        private const string FAMILY_VERSION = "0.1";

        private readonly RemmeApi.IRemmeApi _remmeRest;
        private readonly IRemmeTransactionService _remmeTransactionService;

        public RemmeToken(RemmeApi.IRemmeApi remmeRest, IRemmeTransactionService remmeTransactionService)
        {
            _remmeRest = remmeRest;
            _remmeTransactionService = remmeTransactionService;
        }

        public async Task<ulong> GetBalance(string publicKey)
        {
            ValidatePublicKey(publicKey);

            return await GetBalanceByAddress(REMChainUtils.GetAddressFromData(publicKey, FAMILY_NAME));
        }

        public async Task<ulong> GetBalanceByAddress(string publicKey)
        {
            return await _remmeRest
                .SendRequest<BalanceCheckRequest, ulong>
                (RemmeMethodsEnum.GetBalance,
                    new BalanceCheckRequest { PublicKey = publicKey });
        }
        
        public async Task<BaseTransactionResponse> TransferByAddress(string addressTo, ulong amount)
        {
            var transferProto = GenerateTransferPayload(addressTo, amount);
            var remmeTransaction = _remmeTransactionService.GetTransactionPayload(transferProto, 0);
            var inputsOutputs = _remmeTransactionService.GetDataInputOutput(transferProto.AddressTo);
            var transactionDto = _remmeTransactionService.GenerateTransactionDto(
                remmeTransaction,
                inputsOutputs,
                FAMILY_NAME,
                FAMILY_VERSION);

            var resultTrans = await _remmeTransactionService.CreateTransaction(transactionDto);

            return await _remmeTransactionService.SendTransaction(resultTrans);
        }

        public async Task<BaseTransactionResponse> Transfer(string publicKeyTo, ulong amount)
        {
            ValidatePublicKey(publicKeyTo);

            return await TransferByAddress(REMChainUtils.GetAddressFromData(publicKeyTo, FAMILY_NAME), amount);
        }

        #region Private Helpers       

        private TransferPayload GenerateTransferPayload(string addressTo, ulong amount)
        {
            return new TransferPayload
            {
                AddressTo = addressTo,
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
