using Org.BouncyCastle.Pkcs;
using REMME.Auth.Client.Contracts.Models;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace REMME.Auth.Client.Contracts
{
    interface IRemmeClient
    {
        Task<CertificateTransactionResponse> CreateCertificate(string comonName, string email = null);

        Task<CertificateTransactionResponse> StoreCertificate(Pkcs10CertificationRequest signingRequest);

        Task<bool> CheckCertificate(X509Certificate2 certificate);

    }
}
