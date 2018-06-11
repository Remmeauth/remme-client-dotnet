using Org.BouncyCastle.Pkcs;
using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.Contracts.Models.PyblicKeyStore;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace REMME.Auth.Client.Contracts
{
    /// <summary>
    /// The Interface which includes methods for flexible work with certificates and REMChain
    /// </summary>
    public interface IRemmeCertificate
    {
        /// <summary>
        /// Creates certificate from provided business data
        /// Will create certificate keypair, sign certificate and send 
        /// transaction to store it on REMChain
        /// </summary>
        /// <param name="certificateDataToCreate">Data transfer object with business fields</param>
        /// <returns>Data transfer object with X509Certificate2 result and Event for subscription inside</returns>
        Task<CertificateTransactionResponse> CreateAndStore(CertificateCreateDto certificateDataToCreate);

        /// <summary>
        /// Stores a public key data to REMChain
        /// </summary>
        /// <param name="publicKeyStoreDto">Data transfer object object to get public data from</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResponse> StorePublicKey(PublicKeyStoreDto publicKeyStoreDto);

        /// <summary>
        /// Checks the certificate validity on REMchain
        /// </summary>
        /// <param name="certificate">X509Certificate2 object to get public data from</param>
        /// <returns>True if certificate valid and was not revoked</returns>
        Task<bool> CheckCertificate(X509Certificate2 certificate);

        /// <summary>
        /// Checks the certificate validity on REMchain
        /// </summary>
        /// <param name="certificate">PEM encoded string of cert to get public data from</param>
        /// <returns>True if certificate valid and was not revoked</returns>
        Task<bool> CheckCertificate(string pemEncodedCRT);

        /// <summary>
        /// Checks the certificate validity on REMchain
        /// </summary>
        /// <param name="certificate">Public bytes of cert to get public data from</param>
        /// <returns>True if certificate valid and was not revoked</returns>
        Task<bool> CheckCertificate(byte[] encodedCRT);

        /// <remarks>WILL BE REIMPLEMENTED INSIDE AFTER EXTERNALL API IS FINISHED. INTERFACE WILL BE THE SAME</remarks>
        /// <summary>
        /// <summary>
        /// Revokes the certificate on REMChain
        /// </summary>
        /// <param name="certificate">X509Certificate2 object to get public data from</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResponse> Revoke(X509Certificate2 certificate);

        /// <remarks>WILL BE REIMPLEMENTED INSIDE AFTER EXTERNALL API IS FINISHED. INTERFACE WILL BE THE SAME</remarks>
        /// <summary>
        /// Checks the certificate validity on REMchain
        /// </summary>
        /// <param name="certificate">PEM encoded string of cert to get public data from</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResponse> Revoke(string pemEncodedCRT);

        /// <remarks>WILL BE REIMPLEMENTED INSIDE AFTER EXTERNALL API IS FINISHED. INTERFACE WILL BE THE SAME</remarks>
        /// <summary>
        /// Checks the certificate validity on REMchain
        /// </summary>
        /// <param name="certificate">Public bytes of cert to get public data from</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResponse> Revoke(byte[] encodedCRT);

        /// <summary>
        /// Retrieves the certificates of the specified user
        /// </summary>
        /// <param name="userPublicKey">The public key of the user to get the certificates</param>
        /// <returns>The addresses of certificates for the specified user</returns>
        Task<IEnumerable<string>> GetUserCertificates(string userPublicKey);
    }
}
