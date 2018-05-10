using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REMME.Auth.Client.Contracts;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.RemmeApi;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.X509;
using System.Security.Cryptography;
using SystemX509 = System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.Pkcs;

namespace REMME.Auth.Client
{
    public class RemmeClient : IRemmeClient
    {
        private readonly RemmeRest _remmeRest;
        private const int _rsaKeySize = 4096;

        public RemmeClient(string nodeAddress = "localhost:8080")
        {
            _remmeRest = new RemmeRest(nodeAddress);
        }

        public async Task<SystemX509.X509Certificate2> CreateCertificate(string comonName, string email)
        {
            var subject = CreateSubject(comonName, email);
            var pair = GetKeyPairWithDotNet();
            var keyParams = (RsaPrivateCrtKeyParameters)pair.Private;
            var rsaPrivateKey = DotNetUtilities.ToRSA(keyParams);

            var pkcs10CertificationRequest = CreateSignRequest(subject, pair);

            var cert = await StoreCertificate(pkcs10CertificationRequest);
            cert.PrivateKey = rsaPrivateKey;
            return cert;
        }

        public async Task<SystemX509.X509Certificate2> StoreCertificate(Pkcs10CertificationRequest signingRequest)
        {
            var payload = new CertificateRequestPayload(signingRequest);
            var result = await _remmeRest
                .PutRequest<CertificateRequestPayload, CertificateResult>(
                            payload,
                            RemmeMethodsEnum.CertificateStore);

            return GetCertificateFromResponse(result.CertificatePEM);
        }

        public async Task<bool> CheckCertificate(SystemX509.X509Certificate2 certificate)
        {
            var payload = new CertificateCheckPayload(certificate);

            var result = await _remmeRest
                .PostRequest<CertificateCheckPayload, CertificateCheckResult>(
                            payload,
                            RemmeMethodsEnum.Certificate);

            return result.IsRevoked;
        }

        private Pkcs10CertificationRequest CreateSignRequest(X509Name subject, AsymmetricCipherKeyPair keyPair)
        {
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            var signatureFactory = new Asn1SignatureFactory("SHA512WITHRSA", keyPair.Private, random);
            return new Pkcs10CertificationRequest(signatureFactory, subject, keyPair.Public, null, keyPair.Private);
        }

        private X509Name CreateSubject(string comonName, string email)
        {
            var attributes = new Dictionary<DerObjectIdentifier, string>
            {
                { X509Name.CN, comonName },
                { X509Name.EmailAddress, email }
            };
            return new X509Name(attributes.Keys.ToList(), attributes);
        }

        private AsymmetricCipherKeyPair GenerateKeyPair()
        {
            var rsaKeyPairGenerator = new RsaKeyPairGenerator();
            var genParam = new RsaKeyGenerationParameters
                    (BigInteger.ValueOf(0x10001), new SecureRandom(), (int)1024, 128);

            rsaKeyPairGenerator.Init(genParam);
            return rsaKeyPairGenerator.GenerateKeyPair();
        }

        public static AsymmetricCipherKeyPair GetKeyPairWithDotNet()
        {
            using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(_rsaKeySize))
            {
                RSAParameters rsaKeyInfo = rsaProvider.ExportParameters(true);
                return DotNetUtilities.GetRsaKeyPair(rsaKeyInfo);
            }
        }

        private SystemX509.X509Certificate2 GetCertificateFromResponse(string pemCertificate)
        {
            var pemString = pemCertificate
                .Replace("-----BEGIN CERTIFICATE-----", "")
                .Replace("-----END CERTIFICATE-----", "");
            return new SystemX509.X509Certificate2(Convert.FromBase64String(pemString));
        }

    }
}
