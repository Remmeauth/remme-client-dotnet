using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace REMME.Auth.Client.Contracts.Models
{
    public class CertificateDto
    {
        public X509Certificate2 Certificate { get; set; }
        
        public string CertificatePEM { get => GetPemCertificate();  }

        public AsymmetricCipherKeyPair KeyPair { get; set; }

        public byte[] PublicKeyDer { get => GetAsn1RsaPublicKey().GetDerEncoded(); }

        public byte[] PrivateKeyDer { get => GetAsn1RsaPrivateKey().GetDerEncoded(); }

        public string PublicKeyPem { get => GetPemPublicKey(); }
        public string PrivateKeyPem { get => GetPemPrivateKey(); }

        public RSAParameters RSAParameters { get => GetRsaParameters(); }
        public RSACryptoServiceProvider SystemRSAKey { get => GetCryptoServiceProvider(); }

        public Asn1Object GetAsn1RsaPrivateKey()
        {
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(KeyPair.Private);
            return privateKeyInfo.ToAsn1Object();
        }

        public Asn1Object GetAsn1RsaPublicKey()
        {
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(KeyPair.Public);
            return publicKeyInfo.ToAsn1Object();
        }

        #region Privates

        private string GetPemCertificate()
        {
            return string.Format("-----BEGIN CERTIFICATE-----\n{0}\n-----END CERTIFICATE-----",
                                Convert.ToBase64String(Certificate.Export(X509ContentType.Cert)));
        }

        private RSACryptoServiceProvider GetCryptoServiceProvider()
        {
            var rsaKey = new RSACryptoServiceProvider();
            rsaKey.ImportParameters(GetRsaParameters());

            return rsaKey;
        }

        private RSAParameters GetRsaParameters()
        {
            var keyParams = (RsaPrivateCrtKeyParameters)KeyPair.Private;
            RSAParameters rsaParameters = new RSAParameters();

            rsaParameters.Modulus = keyParams.Modulus.ToByteArrayUnsigned();
            rsaParameters.P = keyParams.P.ToByteArrayUnsigned();
            rsaParameters.Q = keyParams.Q.ToByteArrayUnsigned();
            rsaParameters.DP = keyParams.DP.ToByteArrayUnsigned();
            rsaParameters.DQ = keyParams.DQ.ToByteArrayUnsigned();
            rsaParameters.InverseQ = keyParams.QInv.ToByteArrayUnsigned();
            rsaParameters.D = keyParams.Exponent.ToByteArrayUnsigned();
            rsaParameters.Exponent = keyParams.PublicExponent.ToByteArrayUnsigned();

            return rsaParameters;
        }

        private string GetPemPublicKey()
        {
            return string.Format("-----BEGIN PUBLIC KEY-----\n{0}\n-----END PUBLIC KEY-----",
                                 Convert.ToBase64String(PublicKeyDer));
        }

        private string GetPemPrivateKey()
        {
            return string.Format("-----BEGIN RSA PRIVATE KEY-----\n{0}\n-----END RSA PRIVATE KEY-----",
                                 Convert.ToBase64String(PrivateKeyDer));
        }

        #endregion
    }
}
