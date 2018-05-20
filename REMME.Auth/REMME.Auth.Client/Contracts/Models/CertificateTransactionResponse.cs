using System.Security.Cryptography.X509Certificates;

namespace REMME.Auth.Client.Contracts.Models
{
    public class CertificateTransactionResponse : BaseTransactionResonse
    {
        public CertificateTransactionResponse(string socketAddress)
            : base(socketAddress)
        {
        }

        public X509Certificate2 Certificate { get; set; }
    }
}
