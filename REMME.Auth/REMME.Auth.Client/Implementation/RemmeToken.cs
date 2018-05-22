using REMME.Auth.Client.Contracts;
using System;
using System.Threading.Tasks;
using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.RemmeApi;
using REMME.Auth.Client.RemmeApi.Models.Token;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeToken : IRemmeToken
    {
        private readonly RemmeRest _remmeRest;

        public RemmeToken(RemmeRest remmeRest)
        {
            _remmeRest = remmeRest;
        }

        public async Task<int> GetBalance(string publicKey)
        {
            var result = await _remmeRest.GetRequest<BalanceCheckResult>(RemmeMethodsEnum.Token, publicKey);

            return Convert.ToInt32(result.Balance);
        }

        public async Task<BaseTransactionResponse> Transfer(string publicKeyTo, int amount)
        {
            var payload = new TransferPayload { PublicKeyTo = publicKeyTo, Amount = amount };

            var result = await _remmeRest.PostRequest<TransferPayload, TransferResult>(
                            RemmeMethodsEnum.Token,
                            payload);

            return new BaseTransactionResponse(_remmeRest.SocketAddress) { BatchId = result.BachId };
        }
    }
}
