using Org.BouncyCastle.Pkcs;
using REMME.Auth.Client.Contracts.Models;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace REMME.Auth.Client.Contracts
{
    /// <summary>
    /// The Interface which includes methods for flexible work with certificates and REMChain
    /// </summary>
    public interface IRemmeCertificate
    {
        /// <remarks>IN FIRST VERSION OF LIBRARY IT WILL DO CSR TO REMME NODE.
        /// WILL BE REIMPLEMENTED INSIDE AFTER EXTERNALL API IS FINISHED. INTERFACE WILL BE THE SAME</remarks>
        /// <summary>
        /// Creates certificate from provided business data
        /// Will create certificate keypair, sign certificate and send 
        /// transaction to store it on REMChain
        /// </summary>
        /// <param name="certificateDataToCreate">Data transfer object with business fields</param>
        /// <returns>Data transfer object with X509Certificate2 result and Event for subscription inside</returns>
        Task<CertificateTransactionResponse> CreateAndStoreCertificate(CertificateCreateDto certificateDataToCreate);


        /// <remarks>WILL BE IMPLEMENTED AFTER EXTERNALL API IS FINISHED</remarks>
        /// <summary>
        /// Stores a public part of already signed and provided certificate to REMChain
        /// </summary>
        /// <param name="certificate">X509Certificate2 object to get public data from</param>
        /// <returns>Data transfer object with X509Certificate2 result and Event for subscription inside</returns>
        Task<CertificateTransactionResponse> StoreCertificate(X509Certificate2 certificate);

        /// <remarks>WILL BE IMPLEMENTED AFTER EXTERNALL API IS FINISHED</remarks>
        /// <summary>
        /// Stores a public part of already signed and provided certificate to REMChain
        /// </summary>
        /// <param name="certificate">PEM encoded string of cert to get public data from</param>
        /// <returns>Data transfer object with X509Certificate2 result and Event for subscription inside</returns>
        Task<CertificateTransactionResponse> StoreCertificate(string pemEncodedCRT);

        /// <remarks>WILL BE IMPLEMENTED AFTER EXTERNALL API IS FINISHED</remarks>
        /// <summary>
        /// Stores a public part of already signed and provided certificate to REMChain
        /// </summary>
        /// <param name="certificate">Public bytes of cert to get public data from</param>
        /// <returns>Data transfer object with X509Certificate2 result and Event for subscription inside</returns>
        Task<CertificateTransactionResponse> StoreCertificate(byte[] encodedCRT);


        /// <summary>
        /// Signs and stores a public part of provided certificate signing request to REMChain
        /// </summary>
        /// <param name="pemEncodedCSR">PEM encoded string of certificate signing request</param>
        /// <returns>Data transfer object with X509Certificate2 result and Event for subscription inside</returns>
        Task<CertificateTransactionResponse> SignAndStoreCertificateRequest(string pemEncodedCSR);

        /// <summary>
        /// Signs and stores a public part of provided certificate signing request to REMChain
        /// </summary>
        /// <param name="pemEncodedCSR">Public bytes of certificate signing to get public data from</param>
        /// <returns>Data transfer object with X509Certificate2 result and Event for subscription inside</returns>
        Task<CertificateTransactionResponse> SignAndStoreCertificateRequest(byte[] encodedCSR);

        /// <summary>
        /// Signs and stores a public part of provided certificate signing request to REMChain
        /// </summary>
        /// <param name="pemEncodedCSR">Bouncy Castle Pkcs10CertificationRequest object</param>
        /// <returns>Data transfer object with X509Certificate2 result and Event for subscription inside</returns>
        Task<CertificateTransactionResponse> SignAndStoreCertificateRequest(Pkcs10CertificationRequest csr);


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
        Task<BaseTransactionResonse> Revoke(X509Certificate2 certificate);

        /// <remarks>WILL BE REIMPLEMENTED INSIDE AFTER EXTERNALL API IS FINISHED. INTERFACE WILL BE THE SAME</remarks>
        /// <summary>
        /// Checks the certificate validity on REMchain
        /// </summary>
        /// <param name="certificate">PEM encoded string of cert to get public data from</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResonse> Revoke(string pemEncodedCRT);

        /// <remarks>WILL BE REIMPLEMENTED INSIDE AFTER EXTERNALL API IS FINISHED. INTERFACE WILL BE THE SAME</remarks>
        /// <summary>
        /// Checks the certificate validity on REMchain
        /// </summary>
        /// <param name="certificate">Public bytes of cert to get public data from</param>
        /// <returns>Base transaction response with Event for subscription inside</returns>
        Task<BaseTransactionResonse> Revoke(byte[] encodedCRT);
    }
}
