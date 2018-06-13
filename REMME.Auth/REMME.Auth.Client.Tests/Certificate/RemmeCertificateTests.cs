using Moq;
using NUnit.Framework;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using REMME.Auth.Client.Contracts;
using REMME.Auth.Client.Contracts.Exceptions;
using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.Implementation;
using REMME.Auth.Client.RemmeApi;
using REMME.Auth.Client.RemmeApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace REMME.Auth.Client.Tests.Certificate
{
    [TestFixture]
    public class RemmeCertificateTests
    {
        private const string MOCK_BATCH_ID = "Fake BATCH ID";
        private const string MOCK_EXCEPTION_MESSAGE = "Remme Node exception Message";

        //[Test]
        //public void CreateAndStoreCertificate_NoCN_Provided_ExceptionThrown()
        //{    
        //    // Arrange
        //    var certificateDto = new CertificateCreateDto();
        //    var certificate = new RemmeCertificate(new Mock<IRemmeRest>().Object, new Mock<IRemmeTransactionService>().Object);

        //    //Assert
        //    Assert.That(() => certificate.CreateAndStore(certificateDto),
        //                Throws.TypeOf<ArgumentException>());
        //}

        //[Test]
        //public void CreateAndStoreCertificate_No_Validity_Provided_ExceptionThrown()
        //{
        //    // Arrange
        //    var certificateDto = new CertificateCreateDto() { CommonName = "name"};
        //    var certificate = new RemmeCertificate(new Mock<IRemmeRest>().Object, new Mock<IRemmeTransactionService>().Object);

        //    //Assert
        //    Assert.That(() => certificate.CreateAndStore(certificateDto),
        //                Throws.TypeOf<ArgumentException>());
        //}

        //[Test]
        //public void CheckCertificate_ValidPemDataProvided_StatusReturned()
        //{
        //    // Arrange
        //    var mock = new Mock<IRemmeRest>();
        //    var isRevoked = true;
        //    mock.Setup(a => a.PostRequest<CertificatePayload, CertificateCheckResult>
        //            (It.Is<RemmeMethodsEnum>(t => RemmeMethodsEnum.Certificate == t), It.IsAny<CertificatePayload>()))
        //            .ReturnsAsync(new CertificateCheckResult() { IsRevoked = isRevoked });

        //    var certificate = new RemmeCertificate(mock.Object, new Mock<IRemmeTransactionService>().Object);

        //    //Act
        //    var actualCheckResult = certificate.CheckCertificate(GetMockX509CertPem()).Result;

        //    //Assert
        //    Assert.AreEqual(actualCheckResult, !isRevoked, "CertificateDto status must be returned");
        //}

        ////This test will fail until BatchId will be implemented at REMME REST Revoke
        ////TODO: Remove this comment after implementing
        //[Test]
        //public void Revoke_ValidPemDataProvided_BatchIdReturned()
        //{
        //    // Arrange
        //    var mock = new Mock<IRemmeRest>();
        //    mock.Setup(a => a.DeleteRequest<CertificatePayload, CertificateResult>
        //            (It.Is<RemmeMethodsEnum>(t => RemmeMethodsEnum.Certificate == t), It.IsAny<CertificatePayload>()))
        //            .ReturnsAsync(new CertificateResult() { BachId = MOCK_BATCH_ID });

        //    var certificate = new RemmeCertificate(mock.Object, new Mock<IRemmeTransactionService>().Object);

        //    //Act
        //    var actualCheckResult = certificate.Revoke(GetMockX509CertPem()).Result;

        //    //Assert
        //    Assert.AreEqual(actualCheckResult.BatchId, MOCK_BATCH_ID, "Transaction result object should contain valid batch id");
        //}



        //private Mock<IRemmeRest> GetMockRestSignCertificate(string mockPem, string batchId)
        //{
        //    var mock = new Mock<IRemmeRest>();

        //    mock.Setup(a => a.PutRequest<CertificateRequestPayload, CertificateResult>
        //            (It.Is<RemmeMethodsEnum>(t => RemmeMethodsEnum.CertificateStore == t), It.IsAny<CertificateRequestPayload>()))
        //            .ReturnsAsync(new CertificateResult() { BachId = batchId, CertificatePEM = mockPem });

        //    return mock;
        //}

        #region Helpers

        private string GetMockSubject()
        {
            return "E=user@email.com, SN=Smith, G=John, CN=userName1, C=US, OID.0.9.2342.19200300.100.1.1=023e5841296344b72372eef24d67c8bd4b28121672cd6392e01fa25d40590b9bec, O=REMME";
        }

        private Pkcs10CertificationRequest GetMockCertificateRequest(CertificateCreateDto certificateDataToCreate)
        {
            AsymmetricCipherKeyPair keyPair = null;
            using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(1024))
            {
                RSAParameters rsaKeyInfo = rsaProvider.ExportParameters(true);
                keyPair =  DotNetUtilities.GetRsaKeyPair(rsaKeyInfo);
            }

            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            var signatureFactory = new Asn1SignatureFactory("SHA512WITHRSA", keyPair.Private, random);

            var subject = new Dictionary<DerObjectIdentifier, string>();
            subject.Add(X509Name.EmailAddress, certificateDataToCreate.Email);
            subject.Add(X509Name.Surname, certificateDataToCreate.Surname);
            subject.Add(X509Name.Name, certificateDataToCreate.Name);
            subject.Add(X509Name.CN, certificateDataToCreate.CommonName);
            subject.Add(X509Name.C, certificateDataToCreate.CountryName);

            return new Pkcs10CertificationRequest(signatureFactory, 
                                                  new X509Name(subject.Keys.ToList(), subject), 
                                                  keyPair.Public, null, keyPair.Private);

        }

        private string GetMockX509CertPem()
        {
            return "-----BEGIN CERTIFICATE-----\nMIIEFDCCAvygAwIBAgIUean2nHOAJYkSP9rjE/iHpo+f4I4wDQYJKoZIhvcNAQEL\nBQAwgcMxDjAMBgNVBAoMBVJFTU1FMVIwUAYKCZImiZPyLGQBAQxCMDIzZTU4NDEy\nOTYzNDRiNzIzNzJlZWYyNGQ2N2M4YmQ0YjI4MTIxNjcyY2Q2MzkyZTAxZmEyNWQ0\nMDU5MGI5YmVjMQswCQYDVQQGEwJVUzESMBAGA1UEAwwJdXNlck5hbWUxMQ0wCwYD\nVQQqDARKb2huMQ4wDAYDVQQEDAVTbWl0aDEdMBsGCSqGSIb3DQEJARYOdXNlckBl\nbWFpbC5jb20wHhcNMTgwNTIzMTQ0ODI5WhcNMTkwNTE4MTQ0ODI5WjCBwzEOMAwG\nA1UECgwFUkVNTUUxUjBQBgoJkiaJk/IsZAEBDEIwMjNlNTg0MTI5NjM0NGI3MjM3\nMmVlZjI0ZDY3YzhiZDRiMjgxMjE2NzJjZDYzOTJlMDFmYTI1ZDQwNTkwYjliZWMx\nCzAJBgNVBAYTAlVTMRIwEAYDVQQDDAl1c2VyTmFtZTExDTALBgNVBCoMBEpvaG4x\nDjAMBgNVBAQMBVNtaXRoMR0wGwYJKoZIhvcNAQkBFg51c2VyQGVtYWlsLmNvbTCC\nASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKYpxzdOjvIvjfPnnYCFB0M3\nfflJQuxt+OGdKODumkKdajjuFoTVM44KqFdHoG5z46uDCo9ggDFY6VCnEOVelV8B\n/4IabU/k+Y9b/qqBYuTOhiemEXQwN+ZNVWy2XFGej2bOvphLZDKTfevBJq91p4ld\nR61bEJgUzZBraj2gM5XKnNmCc1TmUDsOdCcTpSZWLSfaeCNuTNk0ncO3kbT8STGL\nu5flhm1iL1IB6AUULyyyKTlcH9yVcvUn5Y+8YNMlKo/biM1A7iodGWN7bzGt3CEu\nT2kUpaNaJmF/3XnXDEn1KBp5MSiMUjNV7LDSKxDY79ggZQl0pIn1I3lbuvSyz5EC\nAwEAATANBgkqhkiG9w0BAQsFAAOCAQEAgj/ds9cgkd/u7IOtBPJ3d4ZRxcvl7sjN\n1prl4j5l2afa8GqRwFB2iYggiBcP6k33C28FH1414WmtXDZU5nnKka5EOUJNZx9K\nTUStBTfjaj+yYm1bm5jPHcveKQJbmwjGGnCH+xxp2agtZVID2UGm1mSGtIfId09D\nEHezzBnK1vZCIvNT2H0I3NpBvcs0i6CWAqEGIP5aItpAxEcE3wYteFMDzX9C4sy6\nC0soLmSG+oOXfEG58y/18RLsj+jpqELGmHPEA72c/iuEwe4YQnNbhUWAiTS+GLjz\nvLFCgyKWf8jWQNYpPuV2xMwMItbrB5NNVro3rnLe9z48l7aN83xFtQ==\n-----END CERTIFICATE-----\n";            
        }

        private string ConvertCsrToPem(Pkcs10CertificationRequest request)
        {
            return string.Format("-----BEGIN CERTIFICATE REQUEST-----\n{0}\n-----END CERTIFICATE REQUEST-----",
                    Convert.ToBase64String(request.GetEncoded()));
        }

        private CertificateCreateDto GetMockCertificateDto()
        {
            return new CertificateCreateDto
            {
                CommonName = "userName1",
                Email = "user@email.com",
                Name = "John",
                Surname = "Smith",
                CountryName = "US",
                ValidityDays = 360
            };
        }

        private string GetMockPEMCertificate()
        {
            return "-----BEGIN CERTIFICATE-----\nMIIEFDCCAvygAwIBAgIUean2nHOAJYkSP9rjE/iHpo+f4I4wDQYJKoZIhvcNAQEL\nBQAwgcMxDjAMBgNVBAoMBVJFTU1FMVIwUAYKCZImiZPyLGQBAQxCMDIzZTU4NDEy\nOTYzNDRiNzIzNzJlZWYyNGQ2N2M4YmQ0YjI4MTIxNjcyY2Q2MzkyZTAxZmEyNWQ0\nMDU5MGI5YmVjMQswCQYDVQQGEwJVUzESMBAGA1UEAwwJdXNlck5hbWUxMQ0wCwYD\nVQQqDARKb2huMQ4wDAYDVQQEDAVTbWl0aDEdMBsGCSqGSIb3DQEJARYOdXNlckBl\nbWFpbC5jb20wHhcNMTgwNTIzMTQ0ODI5WhcNMTkwNTE4MTQ0ODI5WjCBwzEOMAwG\nA1UECgwFUkVNTUUxUjBQBgoJkiaJk/IsZAEBDEIwMjNlNTg0MTI5NjM0NGI3MjM3\nMmVlZjI0ZDY3YzhiZDRiMjgxMjE2NzJjZDYzOTJlMDFmYTI1ZDQwNTkwYjliZWMx\nCzAJBgNVBAYTAlVTMRIwEAYDVQQDDAl1c2VyTmFtZTExDTALBgNVBCoMBEpvaG4x\nDjAMBgNVBAQMBVNtaXRoMR0wGwYJKoZIhvcNAQkBFg51c2VyQGVtYWlsLmNvbTCC\nASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKYpxzdOjvIvjfPnnYCFB0M3\nfflJQuxt+OGdKODumkKdajjuFoTVM44KqFdHoG5z46uDCo9ggDFY6VCnEOVelV8B\n/4IabU/k+Y9b/qqBYuTOhiemEXQwN+ZNVWy2XFGej2bOvphLZDKTfevBJq91p4ld\nR61bEJgUzZBraj2gM5XKnNmCc1TmUDsOdCcTpSZWLSfaeCNuTNk0ncO3kbT8STGL\nu5flhm1iL1IB6AUULyyyKTlcH9yVcvUn5Y+8YNMlKo/biM1A7iodGWN7bzGt3CEu\nT2kUpaNaJmF/3XnXDEn1KBp5MSiMUjNV7LDSKxDY79ggZQl0pIn1I3lbuvSyz5EC\nAwEAATANBgkqhkiG9w0BAQsFAAOCAQEAgj/ds9cgkd/u7IOtBPJ3d4ZRxcvl7sjN\n1prl4j5l2afa8GqRwFB2iYggiBcP6k33C28FH1414WmtXDZU5nnKka5EOUJNZx9K\nTUStBTfjaj+yYm1bm5jPHcveKQJbmwjGGnCH+xxp2agtZVID2UGm1mSGtIfId09D\nEHezzBnK1vZCIvNT2H0I3NpBvcs0i6CWAqEGIP5aItpAxEcE3wYteFMDzX9C4sy6\nC0soLmSG+oOXfEG58y/18RLsj+jpqELGmHPEA72c/iuEwe4YQnNbhUWAiTS+GLjz\nvLFCgyKWf8jWQNYpPuV2xMwMItbrB5NNVro3rnLe9z48l7aN83xFtQ==\n-----END CERTIFICATE-----\n";
        }
        #endregion
    }
}
