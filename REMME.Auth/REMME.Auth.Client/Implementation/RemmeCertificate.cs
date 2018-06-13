using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using REMME.Auth.Client.Contracts;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using REMME.Auth.Client.Contracts.Models;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using SystemX509 = System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Utilities;
using REMME.Auth.Client.Implementation.Utils;
using REMME.Auth.Client.Contracts.Models.PublicKeyStore;
using REMME.Auth.Client.RemmeApi.Models;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeCertificate : IRemmeCertificate
    {
        private const int _rsaKeySize = 2048;
        private const string _signatureAlgorithm = "SHA512WITHRSA";
        private readonly IRemmePublicKeyStorage _remmeKeyStorage;

        public RemmeCertificate(IRemmePublicKeyStorage remmeKeyStorage)
        {
            _remmeKeyStorage = remmeKeyStorage;
        }

        #region Creation 

        public async Task<CertificateTransactionResponse> CreateAndStore(CertificateCreateDto certificateDataToCreate)
        {
            ValidateCreateDto(certificateDataToCreate);

            var subject = CreateSubject(certificateDataToCreate);

            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            var pair = _remmeKeyStorage.GenerateRsaKeyPair(random, _rsaKeySize);
            var certificateDto = CreateCertificate(certificateDataToCreate, pair, random);

            return await Store(certificateDto);
        }

        #endregion

        #region Store Methods

        public async Task<CertificateTransactionResponse> Store(CertificateDto certificateDto)
        {
            var storeResult = await _remmeKeyStorage.Store(GetStoreDtoFromCertificate(certificateDto));

            return new CertificateTransactionResponse(storeResult.SocketAddress)
            {
                BatchId = storeResult.BatchId,
                CertificateDto = certificateDto
            };
        }

        #endregion

        #region Check Methods

        public async Task<PublicKeyCheckResult> Check(SystemX509.X509Certificate2 certificate)
        {
            var publicKeyPem = certificate.GetPemPublicKey();
            return await _remmeKeyStorage.Check(publicKeyPem);
        }

        public async Task<PublicKeyCheckResult> Check(string pemEncodedCRT)
        {            
            return await Check(pemEncodedCRT.SystemX509FromPem());
        }

        public async Task<PublicKeyCheckResult> Check(byte[] encodedCRT)
        {
            return await Check(new SystemX509.X509Certificate2(encodedCRT));
        }

        #endregion

        #region Revocation Methods

        public async Task<BaseTransactionResponse> Revoke(SystemX509.X509Certificate2 certificate)
        {
            var publicKeyPem = certificate.GetPemPublicKey();
            return await _remmeKeyStorage.Revoke(publicKeyPem);
        }

        public async Task<BaseTransactionResponse> Revoke(string pemEncodedCRT)
        {
            return await Revoke(pemEncodedCRT.SystemX509FromPem());        
        }

        public async Task<BaseTransactionResponse> Revoke(byte[] encodedCert)
        {
            return await Revoke(new SystemX509.X509Certificate2(encodedCert));
        }

        #endregion

        #region Helpers

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

        private PublicKeyStoreDto GetStoreDtoFromCertificate(CertificateDto certificateDto)
        {
            return new PublicKeyStoreDto
            {
                KeyPair = certificateDto.KeyPair,
                EntityData = certificateDto.CertificatePEM,
                EntityOwnerType = EntityOwnerTypeEnum.Personal,
                PublicKeyType = PublicKeyTypeEnum.RSA,
                ValidityFrom = GetUnixTime(certificateDto.Certificate.NotBefore),
                ValidityTo = GetUnixTime(certificateDto.Certificate.NotAfter)
            };
        }

        private void ValidateCreateDto(CertificateCreateDto certificateCreateDto)
        {
            if (string.IsNullOrEmpty(certificateCreateDto.CommonName))
                throw new ArgumentException("'Common name' must have value");
            if (certificateCreateDto.ValidityDays == 0)
                throw new ArgumentException("'Validity' must have value");
        }

        private uint GetUnixTime(DateTime dateTime)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var diff = dateTime - origin;
            return (uint)Math.Floor(diff.TotalSeconds);
        }

        #endregion
    }
}
