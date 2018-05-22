using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Security.Cryptography;
using SystemX509 = System.Security.Cryptography.X509Certificates;
using REMME.Auth.Client.RemmeApi.Models;
using REMME.Auth.Client.RemmeApi.Models.Certificate;
using System.Reflection;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeCertificate : IRemmeCertificate
    {
        private readonly RemmeRest _remmeRest;
        private const int _rsaKeySize = 2048;

        public RemmeCertificate(RemmeRest remmeRest)
        {
            _remmeRest = remmeRest;
        }

        #region Creation 

        //TODO: Use Validity options after REMME node REST API will support that
        public async Task<CertificateTransactionResponse> CreateAndStoreCertificate(CertificateCreateDto certificateDataToCreate)
        {
            var subject = CreateSubject(certificateDataToCreate);
            var pair = GetKeyPairWithDotNet();
            var keyParams = (RsaPrivateCrtKeyParameters)pair.Private;
            var rsaPrivateKey = DotNetUtilities.ToRSA(keyParams);

            var pkcs10CertificationRequest = CreateSignRequest(subject, pair);

            var certResponse = await SignAndStoreCertificateRequest(pkcs10CertificationRequest);
            certResponse.Certificate.PrivateKey = rsaPrivateKey;
            return certResponse;
        }

        #endregion

        #region Methods for CSR

        public async Task<CertificateTransactionResponse> SignAndStoreCertificateRequest(Pkcs10CertificationRequest signingRequest)
        {
            var payload = new CertificateRequestPayload(signingRequest);
            var apiResult = await _remmeRest
                .PutRequest<CertificateRequestPayload, CertificateResult>(
                            RemmeMethodsEnum.CertificateStore,
                            payload);

            var result = new CertificateTransactionResponse(_remmeRest.SocketAddress);
            result.BatchId = apiResult.BachId;
            result.Certificate = GetCertificateFromPem(apiResult.CertificatePEM);

            return result;
        }

        public Task<CertificateTransactionResponse> SignAndStoreCertificateRequest(string pemEncodedCSR)
        {
            return SignAndStoreCertificateRequest(GetBytesFromPemCsr(pemEncodedCSR));
        }

        public Task<CertificateTransactionResponse> SignAndStoreCertificateRequest(byte[] encodedCSR)
        {
            return SignAndStoreCertificateRequest(new Pkcs10CertificationRequest(encodedCSR));
        }

        #endregion

        #region Store Methods

        public Task<CertificateTransactionResponse> StoreCertificate(SystemX509.X509Certificate2 certificate)
        {
            throw new NotImplementedException();
        }

        public Task<CertificateTransactionResponse> StoreCertificate(string pemEncodedCRT)
        {
            throw new NotImplementedException();
        }

        public Task<CertificateTransactionResponse> StoreCertificate(byte[] encodedCRT)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Check Methods

        public async Task<bool> CheckCertificate(SystemX509.X509Certificate2 certificate)
        {
            var payload = new CertificatePayload(certificate);

            var result = await _remmeRest
                .PostRequest<CertificatePayload, CertificateCheckResult>(
                            RemmeMethodsEnum.Certificate,
                            payload);

            return result.IsValid;
        }

        public Task<bool> CheckCertificate(string pemEncodedCRT)
        {
            return CheckCertificate(GetCertificateFromPem(pemEncodedCRT));
        }

        public Task<bool> CheckCertificate(byte[] encodedCRT)
        {
            return CheckCertificate(new SystemX509.X509Certificate2(encodedCRT));
        }

        #endregion

        #region Revocation Methods

        //TODO: In current version of REMME REST API there is no result for REVOKE operation
        //While implementing it via external API we should consider getting BATCH ID, so we know 
        //when block will be writen to REMCHain with revoke data
        public async Task<BaseTransactionResponse> Revoke(SystemX509.X509Certificate2 certificate)
        {
            var payload = new CertificatePayload(certificate);

            var result = await _remmeRest
                .DeleteRequest<CertificatePayload, CertificateResult>(
                            RemmeMethodsEnum.Certificate,
                            payload);

            return new BaseTransactionResponse(_remmeRest.SocketAddress);
        }

        public Task<BaseTransactionResponse> Revoke(string pemEncodedCRT)
        {
            return Revoke(GetCertificateFromPem(pemEncodedCRT));
        }

        public Task<BaseTransactionResponse> Revoke(byte[] encodedCRT)
        {
            return Revoke(new SystemX509.X509Certificate2(encodedCRT));
        }

        #endregion

        #region Read
        public async Task<IEnumerable<string>> GetUserCertificates(string userPublicKey)
        {
            var result = await _remmeRest.GetRequest<UserCertificatesResult>(
                                RemmeMethodsEnum.UserCertificates, userPublicKey);

            return result.CertificateAdresses;
        }
        #endregion

        #region Helpers

        private Pkcs10CertificationRequest CreateSignRequest(X509Name subject, AsymmetricCipherKeyPair keyPair)
        {
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            var signatureFactory = new Asn1SignatureFactory("SHA512WITHRSA", keyPair.Private, random);
            return new Pkcs10CertificationRequest(signatureFactory, subject, keyPair.Public, null, keyPair.Private);
        }

        private X509Name CreateSubject(CertificateCreateDto certificateDataToCreate)
        {
            if (string.IsNullOrEmpty(certificateDataToCreate.CommonName))
                throw new ArgumentException("'Common name' must have value");
            if (certificateDataToCreate.Validity == 0)
                throw new ArgumentException("'Validity' must have value");

            //TODO: Refactor this method to look better
            var attributes = new Dictionary<DerObjectIdentifier, string>();
            AddValueToSubject(attributes, X509Name.CN, certificateDataToCreate.CommonName);
            AddValueToSubject(attributes, X509Name.EmailAddress, certificateDataToCreate.Email);
            AddValueToSubject(attributes, X509Name.C, certificateDataToCreate.CountryName);
            AddValueToSubject(attributes, X509Name.L, certificateDataToCreate.LocalityName);
            AddValueToSubject(attributes, X509Name.PostalAddress, certificateDataToCreate.PostalAddress);
            AddValueToSubject(attributes, X509Name.PostalCode, certificateDataToCreate.PostalCode);
            AddValueToSubject(attributes, X509Name.Street, certificateDataToCreate.StreetAddress);
            AddValueToSubject(attributes, X509Name.ST, certificateDataToCreate.StateName);
            AddValueToSubject(attributes, X509Name.Name, certificateDataToCreate.Name);
            AddValueToSubject(attributes, X509Name.Surname, certificateDataToCreate.Surname);
            AddValueToSubject(attributes, X509Name.Pseudonym, certificateDataToCreate.Pseudonym);
            AddValueToSubject(attributes, X509Name.Generation, certificateDataToCreate.GenerationQualifier);
            AddValueToSubject(attributes, X509Name.T, certificateDataToCreate.Title);
            AddValueToSubject(attributes, X509Name.SerialNumber, certificateDataToCreate.Serial);
            AddValueToSubject(attributes, X509Name.BusinessCategory, certificateDataToCreate.BusinessCategory);
            
            return new X509Name(attributes.Keys.ToList(), attributes);
        }

        private void AddValueToSubject(Dictionary<DerObjectIdentifier, string> subject,
                                       DerObjectIdentifier identifier,
                                       string value)
        {

            if (!string.IsNullOrEmpty(value))
                subject.Add(identifier, value);
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

        private byte[] GetBytesFromPemCsr(string pemCsr)
        {
            var pemString = pemCsr
                .Replace("-----BEGIN CERTIFICATE REQUEST-----", "")
                .Replace("-----END CERTIFICATE REQUEST-----", "");
            return Convert.FromBase64String(pemString);
        }

        private SystemX509.X509Certificate2 GetCertificateFromPem(string pemCertificate)
        {
            var pemString = pemCertificate
                .Replace("-----BEGIN CERTIFICATE-----", "")
                .Replace("-----END CERTIFICATE-----", "");
            return new SystemX509.X509Certificate2(Convert.FromBase64String(pemString));
        }

        #endregion
    }
}
