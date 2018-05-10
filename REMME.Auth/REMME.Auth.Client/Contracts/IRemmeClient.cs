using Org.BouncyCastle.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace REMME.Auth.Client.Contracts
{
    interface IRemmeClient
    {
        Task<X509Certificate2> CreateCertificate(string comonName, string email = null);

        Task<X509Certificate2> StoreCertificate(Pkcs10CertificationRequest signingRequest);

        Task<bool> CheckCertificate(X509Certificate2 certificate);

    }
}
