using Moq;
using NUnit.Framework;
using REMME.Auth.Client.Contracts;
using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.Implementation;
using REMME.Auth.Client.RemmeApi.Models;
using System;

namespace REMME.Auth.Client.Tests.Certificate
{
    [TestFixture]
    public class RemmeCertificateTests
    {
        private const string MOCK_BATCH_ID = "Fake BATCH ID";
        private const string MOCK_EXCEPTION_MESSAGE = "Remme Node exception Message";

        [Test]
        public void CreateAndStoreCertificate_NoCN_Provided_ExceptionThrown()
        {
            // Arrange
            var certificateDto = new CertificateCreateDto();
            var certificate = new RemmeCertificate(new Mock<IRemmePublicKeyStorage>().Object);

            //Assert
            Assert.That(() => certificate.CreateAndStore(certificateDto),
                        Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CreateAndStoreCertificate_No_Validity_Provided_ExceptionThrown()
        {
            // Arrange
            var certificateDto = new CertificateCreateDto() { CommonName = "name" };
            var certificate = new RemmeCertificate(new Mock<IRemmePublicKeyStorage>().Object);

            //Assert
            Assert.That(() => certificate.CreateAndStore(certificateDto),
                        Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CheckCertificate_ValidPemDataProvided_StatusReturned()
        {
            // Arrange
            var mock = new Mock<IRemmePublicKeyStorage>();
            var isRevoked = true;
            mock.Setup(a => a.Check(It.IsAny<string>()))
                .ReturnsAsync(new PublicKeyCheckResult() { IsRevoked = isRevoked });

            var certificate = new RemmeCertificate(mock.Object);

            //Act
            var actualCheckResult = certificate.Check(GetMockX509CertPem()).Result;

            //Assert
            Assert.AreEqual(actualCheckResult.IsRevoked, isRevoked, "PublicKeyCheckResult status must be returned");
        }

        [Test]
        public void Revoke_ValidPemDataProvided_BatchIdReturned()
        {
            // Arrange
            var mock = new Mock<IRemmePublicKeyStorage>();
            mock.Setup(a => a.Revoke(It.IsAny<string>()))
                .ReturnsAsync(new BaseTransactionResponse(string.Empty) { BatchId = MOCK_BATCH_ID });

            var certificate = new RemmeCertificate(mock.Object);

            //Act
            var actualRevokeResult = certificate.Revoke(GetMockX509CertPem()).Result;

            //Assert
            Assert.IsTrue(MOCK_BATCH_ID == actualRevokeResult.BatchId, "Transaction result object should contain valid batch id");
        }

        #region Helpers

        private string GetMockSubject()
        {
            return "E=user@email.com, SN=Smith, G=John, CN=userName1, C=US, OID.0.9.2342.19200300.100.1.1=023e5841296344b72372eef24d67c8bd4b28121672cd6392e01fa25d40590b9bec, O=REMME";
        }

        private string GetMockX509CertPem()
        {
            return "-----BEGIN CERTIFICATE-----\nMIIEFDCCAvygAwIBAgIUean2nHOAJYkSP9rjE/iHpo+f4I4wDQYJKoZIhvcNAQEL\nBQAwgcMxDjAMBgNVBAoMBVJFTU1FMVIwUAYKCZImiZPyLGQBAQxCMDIzZTU4NDEy\nOTYzNDRiNzIzNzJlZWYyNGQ2N2M4YmQ0YjI4MTIxNjcyY2Q2MzkyZTAxZmEyNWQ0\nMDU5MGI5YmVjMQswCQYDVQQGEwJVUzESMBAGA1UEAwwJdXNlck5hbWUxMQ0wCwYD\nVQQqDARKb2huMQ4wDAYDVQQEDAVTbWl0aDEdMBsGCSqGSIb3DQEJARYOdXNlckBl\nbWFpbC5jb20wHhcNMTgwNTIzMTQ0ODI5WhcNMTkwNTE4MTQ0ODI5WjCBwzEOMAwG\nA1UECgwFUkVNTUUxUjBQBgoJkiaJk/IsZAEBDEIwMjNlNTg0MTI5NjM0NGI3MjM3\nMmVlZjI0ZDY3YzhiZDRiMjgxMjE2NzJjZDYzOTJlMDFmYTI1ZDQwNTkwYjliZWMx\nCzAJBgNVBAYTAlVTMRIwEAYDVQQDDAl1c2VyTmFtZTExDTALBgNVBCoMBEpvaG4x\nDjAMBgNVBAQMBVNtaXRoMR0wGwYJKoZIhvcNAQkBFg51c2VyQGVtYWlsLmNvbTCC\nASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKYpxzdOjvIvjfPnnYCFB0M3\nfflJQuxt+OGdKODumkKdajjuFoTVM44KqFdHoG5z46uDCo9ggDFY6VCnEOVelV8B\n/4IabU/k+Y9b/qqBYuTOhiemEXQwN+ZNVWy2XFGej2bOvphLZDKTfevBJq91p4ld\nR61bEJgUzZBraj2gM5XKnNmCc1TmUDsOdCcTpSZWLSfaeCNuTNk0ncO3kbT8STGL\nu5flhm1iL1IB6AUULyyyKTlcH9yVcvUn5Y+8YNMlKo/biM1A7iodGWN7bzGt3CEu\nT2kUpaNaJmF/3XnXDEn1KBp5MSiMUjNV7LDSKxDY79ggZQl0pIn1I3lbuvSyz5EC\nAwEAATANBgkqhkiG9w0BAQsFAAOCAQEAgj/ds9cgkd/u7IOtBPJ3d4ZRxcvl7sjN\n1prl4j5l2afa8GqRwFB2iYggiBcP6k33C28FH1414WmtXDZU5nnKka5EOUJNZx9K\nTUStBTfjaj+yYm1bm5jPHcveKQJbmwjGGnCH+xxp2agtZVID2UGm1mSGtIfId09D\nEHezzBnK1vZCIvNT2H0I3NpBvcs0i6CWAqEGIP5aItpAxEcE3wYteFMDzX9C4sy6\nC0soLmSG+oOXfEG58y/18RLsj+jpqELGmHPEA72c/iuEwe4YQnNbhUWAiTS+GLjz\nvLFCgyKWf8jWQNYpPuV2xMwMItbrB5NNVro3rnLe9z48l7aN83xFtQ==\n-----END CERTIFICATE-----\n";
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
        #endregion
    }
}
