using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.RemmeApi.Models.AtomicSwap;
using System.Threading.Tasks;

namespace REMME.Auth.Client.Contracts
{
    public interface IRemmeAtomicSwap
    {
        Task<ITransactionResponse> Init(SwapInitDto swapInitDto);
        Task<ITransactionResponse> Approve(string swapId);
        Task<ITransactionResponse> Expire(string swapId);
        Task<ITransactionResponse> SetSecretLock(string swapId, string secretLock);
        Task<ITransactionResponse> Close(string swapId, string secretKey);
        Task<SwapInfoDto> GetInfo(string swapId);
        Task<string> GetPublicKey();
    }
}
