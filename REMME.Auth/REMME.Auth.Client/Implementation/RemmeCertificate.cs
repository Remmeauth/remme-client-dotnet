﻿using System;
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
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Utilities;
using REMME.Auth.Client.Contracts.Models.PyblicKeyStore;
using REMME.Auth.Client.Crypto;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeCertificate : IRemmeCertificate
    {
        private readonly IRemmeRest _remmeRest;
        private readonly IRemmeTransactionService _remmeTransactionService;

        private const int _rsaKeySize = 2048;
        private const string _signatureAlgorithm = "SHA512WITHRSA";

        public RemmeCertificate(IRemmeRest remmeRest, IRemmeTransactionService remmeTransactionService)
        {
            _remmeRest = remmeRest;
            _remmeTransactionService = remmeTransactionService;
        }

        #region Creation 

        public async Task<CertificateTransactionResponse> CreateAndStore(CertificateCreateDto certificateDataToCreate)
        {
            var subject = CreateSubject(certificateDataToCreate);

            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            var pair = GenerateRsaKeyPair(random);

            var certificateDto = CreateCertificate(certificateDataToCreate, pair, random);

            var storeDto = GetPublicKeyDtoFromCert(certificateDto, pair);
            var storedResult = await StorePublicKey(storeDto);

            return new CertificateTransactionResponse(_remmeRest.SocketAddress)
            {
                CertificateDto = certificateDto,
                BatchId = storedResult.BatchId
            };
        }

        #endregion

        #region Store Methods

        public async Task<BaseTransactionResponse> StorePublicKey(PublicKeyStoreDto publicKeyStoreDto)
        {
            /// TRANSACTION SHOULD BE CREATED AND SENT TO REMCHAIN
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
            var signatureFactory = new Asn1SignatureFactory(_signatureAlgorithm, keyPair.Private, random);
            return new Pkcs10CertificationRequest(signatureFactory, subject, keyPair.Public, null, keyPair.Private);
        }

        private X509Name CreateSubject(CertificateCreateDto certificateDataToCreate)
        {
            //TODO: Refactor this method to look better
            var attributes = new Dictionary<DerObjectIdentifier, string>();
            AddValueToSubject(attributes, X509Name.CN, certificateDataToCreate.CommonName);
            AddValueToSubject(attributes, X509Name.O, certificateDataToCreate.OrganizationName);
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

        private CertificateDto CreateCertificate(CertificateCreateDto certificateCreateDto,
                                                 AsymmetricCipherKeyPair rsaKeyPair,
                                                 SecureRandom random)
        {
            ValidateCreateDto(certificateCreateDto);

            var certificateGenerator = new X509V3CertificateGenerator();
            var signatureFactory = new Asn1SignatureFactory(_signatureAlgorithm, rsaKeyPair.Private, random);
            var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);

            var subject = CreateSubject(certificateCreateDto);
            certificateGenerator.SetIssuerDN(subject);
            certificateGenerator.SetSubjectDN(subject);
            certificateGenerator.SetSerialNumber(serialNumber);
            certificateGenerator.SetNotBefore(certificateCreateDto.NotBefore);
            certificateGenerator.SetNotAfter(certificateCreateDto.NotAfter);
            certificateGenerator.SetPublicKey(rsaKeyPair.Public);

            var certificate = certificateGenerator.Generate(signatureFactory);
            return new CertificateDto
            {
                Certificate = new SystemX509.X509Certificate2(certificate.GetEncoded()),
                PrivateKeyDer = GetAsn1RsaPrivateKey(rsaKeyPair).GetDerEncoded(),
                PublicKeyDer = GetAsn1RsaPublicKey(rsaKeyPair).GetDerEncoded()
            };
        }

        private Asn1Object GetAsn1RsaPrivateKey(AsymmetricCipherKeyPair keyPair)
        {
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
            return privateKeyInfo.ToAsn1Object();
        }

        private Asn1Object GetAsn1RsaPublicKey(AsymmetricCipherKeyPair keyPair)
        {
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);
            return publicKeyInfo.ToAsn1Object();
        }

        private void AddValueToSubject(Dictionary<DerObjectIdentifier, string> subject,
                                       DerObjectIdentifier identifier,
                                       string value)
        {

            if (!string.IsNullOrEmpty(value))
                subject.Add(identifier, value);
        }

        private AsymmetricCipherKeyPair GenerateRsaKeyPair(SecureRandom random)
        {
            var keyGenerationParameters = new KeyGenerationParameters(random, _rsaKeySize);
            var rsaKeyPairGnr = new RsaKeyPairGenerator();
            rsaKeyPairGnr.Init(keyGenerationParameters);

            return rsaKeyPairGnr.GenerateKeyPair();
        }

        private void ValidateCreateDto(CertificateCreateDto certificateCreateDto)
        {
            if (string.IsNullOrEmpty(certificateCreateDto.CommonName))
                throw new ArgumentException("'Common name' must have value");
            if (certificateCreateDto.ValidityDays == 0)
                throw new ArgumentException("'Validity' must have value");
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

        private byte[] GetCertificateHash(SystemX509.X509Certificate2 certificate)
        {
            return certificate
                        .Export(SystemX509.X509ContentType.Cert)
                        .Sha512Digest();
        }

        private string PublicKeyToPem(byte[] key)
        {
            return string.Format(
                "-----BEGIN PUBLIC KEY-----\n{0}\n-----END PUBLIC KEY-----",
                Convert.ToBase64String(key));
        }

        private byte[] SignHashWithKey(AsymmetricCipherKeyPair keyPair, byte[] data)
        {
            var keyParams = (RsaPrivateCrtKeyParameters)keyPair.Private;
            RSAParameters rsaParameters = new RSAParameters();

            rsaParameters.Modulus = keyParams.Modulus.ToByteArrayUnsigned();
            rsaParameters.P = keyParams.P.ToByteArrayUnsigned();
            rsaParameters.Q = keyParams.Q.ToByteArrayUnsigned();
            rsaParameters.DP = keyParams.DP.ToByteArrayUnsigned();
            rsaParameters.DQ = keyParams.DQ.ToByteArrayUnsigned();
            rsaParameters.InverseQ = keyParams.QInv.ToByteArrayUnsigned();
            rsaParameters.D = keyParams.Exponent.ToByteArrayUnsigned();
            rsaParameters.Exponent = keyParams.PublicExponent.ToByteArrayUnsigned();

            RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(_rsaKeySize);
            rsaKey.ImportParameters(rsaParameters);

            return rsaKey.SignHash(data, "SHA512");
        }

        private uint GetUnixTime(DateTime dateTime)
        {            
            return (uint)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        private PublicKeyStoreDto GetPublicKeyDtoFromCert(CertificateDto certificateDto, AsymmetricCipherKeyPair pair)
        {
            var certHash = GetCertificateHash(certificateDto.Certificate);
            var hashSignature = SignHashWithKey(pair, certHash);
            var exponent = ((RsaPrivateCrtKeyParameters)pair.Private).PublicExponent.LongValue;
            return new PublicKeyStoreDto
            {
                EntityHash = certHash.BytesToHexString(),
                EntityHashSignature = hashSignature.BytesToHexString(),
                EntityOwnerType = EntityOwnerTypeEnum.Personal,
                PublicKeyType = PublicKeyTypeEnum.RSA,
                PublicKeyPem = PublicKeyToPem(certificateDto.PublicKeyDer),
                RsaKeySize = _rsaKeySize,
                RsaPublicExponent = exponent,
                ValidityFrom = GetUnixTime(certificateDto.Certificate.NotBefore),
                ValidityTo = GetUnixTime(certificateDto.Certificate.NotAfter)
            };
        }

        #endregion
    }
}
