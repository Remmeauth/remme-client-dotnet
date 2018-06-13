using Org.BouncyCastle.Crypto;
using REMME.Auth.Client.Implementation.Utils;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace REMME.Auth.Client.Contracts.Models
{
    public class CertificateDto
    {

        public X509Certificate2 Certificate { get; set; }
        public AsymmetricCipherKeyPair KeyPair { get; set; }

        public string CertificatePEM { get => Certificate.ToPemString(); }

        public byte[] PublicKeyDer { get => KeyPair.GetPublicKeyBytes(); }

        public byte[] PrivateKeyDer { get => KeyPair.GetPrivateKeyBytes(); }

        public string PublicKeyPem { get => KeyPair.GetPublicKeyPem(); }
        public string PrivateKeyPem { get => KeyPair.GetPrivateKeyPem(); }

        public RSAParameters RSAParameters { get => KeyPair.GetPrivateRsaParameters(); }
        public RSACryptoServiceProvider SystemRSAKey { get => KeyPair.GetCryptoServiceProvider(); }        
    }
}
