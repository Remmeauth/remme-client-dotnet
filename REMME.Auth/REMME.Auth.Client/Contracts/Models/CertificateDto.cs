using System.Security.Cryptography.X509Certificates;

namespace REMME.Auth.Client.Contracts.Models
{
    public class CertificateDto
    {
        public X509Certificate2 Certificate { get; set; }

        public byte[] PublicKeyDer { get; set; }

        public byte[] PrivateKeyDer { get; set; }
    }
}
