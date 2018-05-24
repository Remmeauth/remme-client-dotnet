using REMME.Auth.Client.Contracts;
using System;
using System.Threading.Tasks;
using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.RemmeApi;
using REMME.Auth.Client.RemmeApi.Models.Token;
using System.Text.RegularExpressions;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeToken : IRemmeToken
    {
        private readonly IRemmeRest _remmeRest;

        public RemmeToken(IRemmeRest remmeRest)
        {
            _remmeRest = remmeRest;
        }

        public async Task<int> GetBalance(string publicKey)
        {
            if (!IsValidPublicKey(publicKey))
                throw new ArgumentException("Public key has invalid format");

            var result = await _remmeRest.GetRequest<BalanceCheckResult>(RemmeMethodsEnum.Token, publicKey);

            return Convert.ToInt32(result.Balance);
        }

        public async Task<BaseTransactionResponse> Transfer(string publicKeyTo, long amount)
        {
            //TODO: Move validation to separate entity
            if (amount <= 0)
                throw new ArgumentException("Amount should be positive integer");
            if (!IsValidPublicKey(publicKeyTo))
                throw new ArgumentException("Public key has invalid format");

            var payload = new TransferPayload { PublicKeyTo = publicKeyTo, Amount = amount };

            var result = await _remmeRest.PostRequest<TransferPayload, TransferResult>(
                            RemmeMethodsEnum.Token,
                            payload);

            return new BaseTransactionResponse(_remmeRest.SocketAddress) { BatchId = result.BachId };
        }

        private bool IsValidPublicKey(string publicKey)
        {
            var patern = @"^[0-9a-f]{66}$";
            return new Regex(patern, RegexOptions.IgnoreCase).IsMatch(publicKey);
        }
    }
}
