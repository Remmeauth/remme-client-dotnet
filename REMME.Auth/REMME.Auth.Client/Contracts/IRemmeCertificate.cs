using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.RemmeApi.Models;
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
        /// transaction to store it's public key on REMChain
        /// </summary>
        /// <param name="certificateDataToCreate">Data transfer object with business fields</param>
        /// <returns>Data transfer object with X509Certificate2 with keys result and Event for subscription inside</returns>
        Task<CertificateTransactionResponse> CreateAndStore(CertificateCreateDto certificateDataToCreate);

        /// <summary>
        /// Stores a public key of already signed and provided certificate to REMChain        
        /// </summary>
        /// <param name="certificateDto">Data transfer object to get public data from and keys for signing data hash</param>
        /// <returns>Data transfer object with X509Certificate2 with keys result and Event for subscription inside</returns>
        Task<CertificateTransactionResponse> Store(CertificateDto certificateDto);

        /// <summary>
        /// Checks the certificate validity on REMchain
        /// </summary>
        /// <param name="certificate">X509Certificate2 object to get public data from</param>
        /// <returns>Data transfer object with validity information (valid dates, owner, revoke status)</returns>
        Task<PublicKeyCheckResult> Check(X509Certificate2 certificate);

        /// <summary>
        /// Checks the certificate validity on REMchain
        /// </summary>
        /// <param name="pemEncodedCRT">PEM encoded string of cert to get public data from</param>
        /// <returns>Data transfer object with validity information (valid dates, owner, revoke status)</returns>
        Task<PublicKeyCheckResult> Check(string pemEncodedCRT);

        /// <summary>
        /// Checks the certificate validity on REMchain
        /// </summary>
        /// <param name="encodedCRT">Public bytes of cert to get public data from</param>
        /// <returns>Data transfer object with validity information (valid dates, owner, revoke status)</returns>
        Task<PublicKeyCheckResult> Check(byte[] encodedCRT);

        /// <summary>
        /// <summary>
        /// Revokes the certificate on REMChain
        /// </summary>
        /// <param name="certificate">X509Certificate2 object to get public data from</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResponse> Revoke(X509Certificate2 certificate);

        /// <summary>
        /// Revokes the certificate on REMChain
        /// </summary>
        /// <param name="pemEncodedCRT">PEM encoded string of cert to get public data from</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResponse> Revoke(string pemEncodedCRT);

        /// <summary>
        /// Revokes the certificate on REMChain
        /// </summary>
        /// <param name="encodedCRT">Public bytes of cert to get public data from</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResponse> Revoke(byte[] encodedCRT);
    }
}
