﻿using REMME.Auth.Client.Contracts.Models;
using System.Threading.Tasks;

namespace REMME.Auth.Client.Contracts
{
    /// <summary>
    /// The Interface which includes methods for token operations on REMChain account
    /// </summary>
    public interface IRemmeToken
    {
        /// <summary>
        /// Transfers tokens to specific address
        /// </summary>
        /// <param name="publicKeyTo">REMChain public key to send token</param>
        /// <param name="amount">Amount of tokens to send</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResponse> Transfer(string addressTo, ulong amount);
        
        /// <summary>
        /// Transfers tokens to specific address
        /// </summary>
        /// <param name="publicKeyTo">REMChain address to send token</param>
        /// <param name="amount">Amount of tokens to send</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResponse> TransferByAddress(string addressTo, ulong amount);

        /// <summary>
        /// Gets the balance of requested account by public key
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns>Integer representing REM balance</returns>
        Task<ulong> GetBalance(string publicKey);
        
        /// <summary>
        /// Gets the balance of requested account
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns>Integer representing REM balance</returns>
        Task<ulong> GetBalanceByAddress(string publicKey);
    }
}
