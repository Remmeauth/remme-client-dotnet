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

namespace REMME.Auth.Client
{
    public class RemmeClient : IRemmeClient
    {
        public async Task CreateCertificate()
        {
            var pair = GenerateKeyPair();
            var subject = CreateFakeSubject();
            var pkcs10CertificationRequest = CreateSignRequest(subject, pair);

            await StoreCertificate(pkcs10CertificationRequest);
            var pkInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(pair.Private);
            var privateKey = Convert.ToBase64String(pkInfo.GetDerEncoded());
        }

        public async Task StoreCertificate(Pkcs10CertificationRequest signingRequest)
        {
            string url = "http://192.168.99.101:8080/api/v1/certificate/store";
            var payload = new CertificateRequestPayload(signingRequest);
            var result = await new RemmeRest().PutRequest<CertificateRequestPayload, CertificateResult>(url, payload);
        }

        public Pkcs10CertificationRequest CreateSignRequest(X509Name subject, AsymmetricCipherKeyPair keyPair)
        {
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            var signatureFactory = new Asn1SignatureFactory("SHA512WITHRSA", keyPair.Private, random);

            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate = pem.ReadCertificateFromFile("certificate.cer");
            
            return new Pkcs10CertificationRequest(signatureFactory, subject, keyPair.Public, null, keyPair.Private);
        }

        private X509Name CreateFakeSubject()
        {
            var attributes = new Dictionary<DerObjectIdentifier, string>
            {
                { X509Name.CN, "tolik" },
            };
            attributes.Add(X509Name.EmailAddress, "tolik@tolik.tolik");
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
    }
}
