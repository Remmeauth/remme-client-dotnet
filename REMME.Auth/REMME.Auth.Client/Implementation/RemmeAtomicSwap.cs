using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using REMME.Auth.Client.Contracts;
using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.RemmeApi;
using REMME.Auth.Client.RemmeApi.Models.AtomicSwap;
using REMME.Auth.Client.RemmeApi.Models.Proto;
using REMME.Auth.Client.Implementation.Utils;
using Google.Protobuf;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeAtomicSwap : IRemmeAtomicSwap
    {
        private const string FAMILY_NAME = "AtomicSwap";
        private const string FAMILY_VERSION = "0.1";
        private readonly string ZERO_ADDRESS = new String('0', 70);
        private const string SWAP_COMISSION = "0000007ca83d6bbb759da9cde0fb0dec1400c55cc3bbcd6b1243b2e3b0c44298fc1c14";
        private const string BLOCK_INFO_NAMESPACE_ADDRESS = "00b10c00";
        private readonly string BLOCK_INFO_CONFIG_ADDRESS = "00b10c01";


        private readonly RemmeApi.IRemmeApi _remmeRest;
        private readonly IRemmeTransactionService _remmeTransactionService;

        public RemmeAtomicSwap(RemmeApi.IRemmeApi remmeRest, IRemmeTransactionService remmeTransactionService)
        {
            _remmeRest = remmeRest;
            _remmeTransactionService = remmeTransactionService;
        }

        public async Task<ITransactionResponse> Init(SwapInitDto swapInitDto)
        {
            ValidateSwapInitData(swapInitDto);
            var swapInitProto = GenerateAtomicSwapInitPayload(swapInitDto);
            return await SendTransaction(swapInitProto, swapInitProto.SwapId, AtomicSwapMethodEnum.Init);
        }

        public async Task<ITransactionResponse> Approve(string swapId)
        {
            var approveProto = new AtomicSwapApprovePayload { SwapId = swapId };
            return await SendTransaction(approveProto, swapId, AtomicSwapMethodEnum.Approve);
        }

        public async Task<ITransactionResponse> Expire(string swapId)
        {
            var expireProto = new AtomicSwapExpirePayload { SwapId = swapId };
            return await SendTransaction(expireProto, swapId, AtomicSwapMethodEnum.Expire);
        }

        public async Task<ITransactionResponse> SetSecretLock(string swapId, string secretLock)
        {
            var setLockProto = new AtomicSwapSetSecretLockPayload { SwapId = swapId, SecretLock = secretLock };
            return await SendTransaction(setLockProto, swapId, AtomicSwapMethodEnum.SetSecretLock);
        }

        public async Task<ITransactionResponse> Close(string swapId, string secretKey)
        {
            var closeProto = new AtomicSwapClosePayload { SwapId = swapId, SecretKey = secretKey };
            var swapInfo = await GetInfo(swapId);
            return await SendTransaction(closeProto, swapId, AtomicSwapMethodEnum.Close, swapInfo.ReceiverAddress);
        }

        public async Task<SwapInfoDto> GetInfo(string swapId)
        {
            return await _remmeRest
                .SendRequest<GetAtomicSwapInfoRequest, SwapInfoDto>
                (RemmeMethodsEnum.GetAtomicSwapInfo,
                 new GetAtomicSwapInfoRequest { SwapId = swapId });
        }

        public async Task<string> GetPublicKey()
        {
            return await _remmeRest
                .SendRequest<string>
                (RemmeMethodsEnum.GetAtomicSwapPublicKey);
        }

        #region Private Helpers

        private async Task<BaseTransactionResponse> SendTransaction(IMessage message,
                                                                    string swapId,
                                                                    AtomicSwapMethodEnum method,
                                                                    string receiverAddress = "")
        {
            var remmeTransaction = _remmeTransactionService.GetTransactionPayload(message, (uint)method);
            var inputsOutputs = GetInputsOutputs(swapId, method, receiverAddress);
            var transactionDto = _remmeTransactionService.GenerateTransactionDto(
                                                                remmeTransaction,
                                                                inputsOutputs,
                                                                FAMILY_NAME,
                                                                FAMILY_VERSION);

            var resultTrans = await _remmeTransactionService.CreateTransaction(transactionDto);

            return await _remmeTransactionService.SendTransaction(resultTrans);
        }

        private List<string> GetInputsOutputs(string swapId, AtomicSwapMethodEnum method, string receiverAddress = "")
        {
            var result = new List<string>();
            result.Add(REMChainUtils.GetAddressFromData(swapId, FAMILY_NAME));
            result.Add(_remmeTransactionService.SignerAddress);

            switch (method)
            {
                case AtomicSwapMethodEnum.Init:
                    result.Add(ZERO_ADDRESS);
                    result.Add(SWAP_COMISSION);
                    result.Add(BLOCK_INFO_CONFIG_ADDRESS);
                    result.Add(BLOCK_INFO_NAMESPACE_ADDRESS);
                    break;
                case AtomicSwapMethodEnum.Expire:
                    result.Add(ZERO_ADDRESS);
                    result.Add(BLOCK_INFO_CONFIG_ADDRESS);
                    result.Add(BLOCK_INFO_NAMESPACE_ADDRESS);
                    break;
                case AtomicSwapMethodEnum.Close:
                    result.Add(ZERO_ADDRESS);
                    result.Add(receiverAddress);
                    break;
                default:
                    break;
            }

            return result;
        }

        private AtomicSwapInitPayload GenerateAtomicSwapInitPayload(SwapInitDto data)
        {
            return new AtomicSwapInitPayload
            {
                Amount = data.Amount,
                CreatedAt = GetUnixTime(data.CreatedAt),
                EmailAddressEncryptedByInitiator = data.EmailAddressEncryptedByInitiator,
                ReceiverAddress = data.ReceiverAddress,
                SenderAddressNonLocal = data.SenderAddress,
                SecretLockBySolicitor = data.SecretLockBySolicitor,
                SwapId = data.SwapId
            };
        }

        private void ValidateSwapInitData(SwapInitDto data)
        {
            if (!IsValidReceiverAddress(data.ReceiverAddress))
                throw new ArgumentException("Receiver Address has invalid format");
            if (!IsValidSecretLock(data.SecretLockBySolicitor))
                throw new ArgumentException("Secret Lock has invalid format");
        }

        private bool IsValidSecretLock(string secretLock)
        {
            return IsMatchRegex(secretLock, @"^[0-9a-f]{64}$");
        }

        private bool IsValidReceiverAddress(string receiverAddress)
        {
            return IsMatchRegex(receiverAddress, @"^[0-9a-f]{70}$");
        }

        private bool IsMatchRegex(string data, string regexPattern)
        {
            return new Regex(regexPattern, RegexOptions.IgnoreCase).IsMatch(data);
        }

        private uint GetUnixTime(DateTime dateTime)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var diff = dateTime - origin;
            return (uint)Math.Floor(diff.TotalSeconds);
        }

        #endregion
    }
}
