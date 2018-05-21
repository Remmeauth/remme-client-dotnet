using REMME.Auth.Client.Contracts.Models;
using System.Threading.Tasks;

namespace REMME.Auth.Client.Contracts
{
    /// <summary>
    /// The Interface which includes methods for token operations on REMChain account
    /// </summary>
    public interface IRemmeToken
    {
        /// <remarks>WILL BE REIMPLEMENTED INSIDE AFTER EXTERNALL API IS FINISHED. INTERFACE WILL BE THE SAME</remarks>
        /// <summary>
        /// Transfers tokens to specific address
        /// </summary>
        /// <param name="publicKeyTo">REMChain address to send token</param>
        /// <param name="amount">Amount of tokens to send</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResponse> Transfer(string publicKeyTo, int amount);

        /// <summary>
        /// Gets the balance of requested account
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns>Integer representing REM balance</returns>
        Task<int> GetBalance(string publicKey);
    }
}
