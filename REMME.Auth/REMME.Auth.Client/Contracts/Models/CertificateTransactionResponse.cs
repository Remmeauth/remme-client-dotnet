using System.Security.Cryptography.X509Certificates;

namespace REMME.Auth.Client.Contracts.Models
{
    public class CertificateTransactionResponse : BaseTransactionResponse
    {
        public CertificateTransactionResponse(string socketAddress)
            : base(socketAddress)
        {
        }

        public CertificateDto CertificateDto { get; set; }
    }
}
