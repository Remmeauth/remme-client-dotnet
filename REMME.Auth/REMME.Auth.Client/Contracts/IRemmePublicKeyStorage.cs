using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.Contracts.Models.PublicKeyStore;
using REMME.Auth.Client.RemmeApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REMME.Auth.Client.Contracts
{    
    /// <summary>
     /// The Interface which includes methods for flexible work with public keys and REMChain
     /// </summary>
    public interface IRemmePublicKeyStorage
    {
        /// <summary>
        /// Stores a public key, it's hash and validity options to REMChain        
        /// </summary>
        /// <param name="certificateDto">Data transfer object to get public data from and keys for signing data hash</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResponse> Store(PublicKeyStoreDto publicKeyDto);

        /// <summary>
        /// Checks the public key validity on REMchain
        /// </summary>
        /// <param name="pemPublicKey">public key address</param>
        /// <returns>Data transfer object with validity information (valid dates, owner, revoke status)</returns>
        Task<PublicKeyCheckResult> CheckByAddress(string publicKeyAddress);

        /// <summary>
        /// Checks the public key validity on REMchain
        /// </summary>
        /// <param name="pemPublicKey">PEM encoded string of public key</param>
        /// <returns>Data transfer object with validity information (valid dates, owner, revoke status)</returns>
        Task<PublicKeyCheckResult> Check(string pemPublicKey);

        /// <summary>
        /// Checks the public key validity on REMchain
        /// </summary>
        /// <param name="encodedPublicKey">Public key bytes</param>
        /// <returns>Data transfer object with validity information (valid dates, owner, revoke status)</returns>
        Task<PublicKeyCheckResult> Check(byte[] encodedPublicKey);

        /// <summary>
        /// Revokes the public key on REMChain
        /// </summary>
        /// <param name="pemPublicKey">public key address</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResponse> RevokeByAddress(string publicKeyAddress);

        /// <summary>
        /// Revokes the public key on REMChain
        /// </summary>
        /// <param name="pemPublicKey">PEM encoded string of public key</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResponse> Revoke(string pemPublicKey);

        /// <summary>
        /// Revokes the public key on REMChain
        /// </summary>
        /// <param name="encodedPublicKey">Public key bytes</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResponse> Revoke(byte[] encodedPublicKey);

        /// <summary>
        /// Retrieves the public kyes of the specified user
        /// </summary>
        /// <param name="userAccountPublicKey">The public key of the user to get the certificates</param>
        /// <returns>The addresses of public kyes stored on REMChain for the specified user</returns>
        Task<IEnumerable<string>> GetAccountStoredPublicKeys(string userAccountPublicKey);

        /// <summary>
        /// Generates new Rsa pair from provided size
        /// </summary>
        /// <param name="rsaKeySize">Rsa Key Size</param>
        /// <returns>Bouncy Castle usable Rsa keypair object</returns>
        AsymmetricCipherKeyPair GenerateRsaKeyPair(int rsaKeySize);

        /// <summary>
        /// Generates new Rsa pair from provided size
        /// </summary>
        /// <param name="rsaKeySize">Rsa Key Size</param>
        /// <param name="random">Cryptographically strong random number generator</param>
        /// <returns>Bouncy Castle usable Rsa keypair object</returns>
        AsymmetricCipherKeyPair GenerateRsaKeyPair(SecureRandom random, int rsaKeySize);
    }
}
