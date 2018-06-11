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
using SystemX509 = System.Security.Cryptography.X509Certificates;
using REMME.Auth.Client.RemmeApi.Models;
using REMME.Auth.Client.RemmeApi.Models.Certificate;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Utilities;
using REMME.Auth.Client.Crypto;
using REMME.Auth.Client.Contracts.Models.PublicKeyStore;
using REMME.Auth.Client.RemmeApi.Models.Proto;
using Google.Protobuf;
using System.Security.Cryptography.X509Certificates;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeCertificate : IRemmeCertificate
    {
        private const string FAMILY_NAME = "certificate";
        private const string FAMILY_VERSION = "0.1";
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
            return await Revoke(certificate.Export(X509ContentType.Cert));
        }

        public async Task<BaseTransactionResponse> Revoke(string pemEncodedCRT)
        {
            var revokeProto = GenerateRevokePayload(pemEncodedCRT);
            var remmeTransaction = GenerateRevokeRemmeTransaction(revokeProto);
            var inputsOutputs = _remmeTransactionService.GetDataInputOutput(revokeProto.Address);
            var transactionDto = _remmeTransactionService.GenerateTransactionDto(
                                                                remmeTransaction,
                                                                inputsOutputs,
                                                                FAMILY_NAME,
                                                                FAMILY_VERSION);

            var resultTrans = await _remmeTransactionService.CreateTransaction(transactionDto);

            return await _remmeTransactionService.SendTransaction(resultTrans);
        }

        public async Task<BaseTransactionResponse> Revoke(byte[] encodedCRT)
        {
            var pemData = string.Format("-----BEGIN CERTIFICATE-----\n{0}\n-----END CERTIFICATE-----",
                                    Convert.ToBase64String(encodedCRT));
            return await Revoke(pemData);
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
                KeyPair = rsaKeyPair
            };
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

        private SystemX509.X509Certificate2 GetCertificateFromPem(string pemCertificate)
        {
            var pemString = pemCertificate
                .Replace("-----BEGIN CERTIFICATE-----", "")
                .Replace("-----END CERTIFICATE-----", "");
            return new SystemX509.X509Certificate2(Convert.FromBase64String(pemString));
        }        

        private string PublicKeyToPem(byte[] key)
        {
            return string.Format(
                "-----BEGIN PUBLIC KEY-----\n{0}\n-----END PUBLIC KEY-----",
                Convert.ToBase64String(key));
        }

        private uint GetUnixTime(DateTime dateTime)
        {            
            return (uint)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        private PublicKeyStoreDto GetPublicKeyDtoFromCert(CertificateDto certificateDto, AsymmetricCipherKeyPair pair)
        {
            var certHash = certificateDto.CertificatePEM.Sha512Digest();
            var hashSignature = certificateDto.SystemRSAKey.SignHash(certHash, "SHA512");
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

        private RevokeCertificatePayload GenerateRevokePayload(string certificatePem)
        {
            return new RevokeCertificatePayload
            {
                Address = Utils.GetAddressFromData(certificatePem, FAMILY_NAME)
            };
        }

        private TransactionPayload GenerateRevokeRemmeTransaction(RevokeCertificatePayload revokePayload)
        {
            return new TransactionPayload
            {
                Method = 1,
                Data = revokePayload.ToByteString()
            };
        }
        
        #endregion
    }
}
