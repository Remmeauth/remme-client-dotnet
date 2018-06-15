using Moq;
using NUnit.Framework;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using REMME.Auth.Client.Contracts;
using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.Implementation;
using REMME.Auth.Client.RemmeApi;
using REMME.Auth.Client.RemmeApi.Models;
using REMME.Auth.Client.RemmeApi.Models.Proto;

namespace REMME.Auth.Client.Tests.Certificate
{
    [TestFixture]
    public class RemmePublicKeysTests
    {
        private const string MOCK_BATCH_ID = "Fake BATCH ID";
        private const string MOCK_EXCEPTION_MESSAGE = "Remme Node exception Message";

        [Test]
        public void Check_ValidPemDataProvided_StatusReturned()
        {
            // Arrange
            var mock = new Mock<IRemmeRest>();
            var isRevoked = true;
            mock.Setup(a => a.PostRequest<PublicKeyCheckPayload, PublicKeyCheckResult>
                    (It.Is<RemmeMethodsEnum>(t => RemmeMethodsEnum.PublicKey == t), It.IsAny<PublicKeyCheckPayload>()))
                    .ReturnsAsync(new PublicKeyCheckResult() { IsRevoked = isRevoked });

            var publicKey = new RemmePublicKeyStorage(mock.Object, new Mock<IRemmeTransactionService>().Object);

            //Act
            var actualCheckResult = publicKey.Check(GetPublicKeyPem()).Result;

            //Assert
            Assert.AreEqual(actualCheckResult.IsRevoked, isRevoked, "Valid publicKeyDto must be returned");
        }


        [Test]
        public void Revoke_ValidPemDataProvided_BatchIdReturned()
        {
            // Arrange
            var mock = new Mock<IRemmeTransactionService>();
            mock.Setup(a => a.SendTransaction(It.IsAny<Transaction>()))
                    .ReturnsAsync(new BaseTransactionResponse(string.Empty) { BatchId = MOCK_BATCH_ID });

            var publicKey = new RemmePublicKeyStorage(new Mock<IRemmeRest>().Object, mock.Object);

            //Act
            var actualCheckResult = publicKey.Revoke(GetPublicKeyPem()).Result;

            //Assert
            Assert.AreEqual(actualCheckResult.BatchId, MOCK_BATCH_ID, "Transaction result object should contain valid batch id");
        }

        #region Helpers

        private string GetPublicKeyPem()
        {
            return "-----BEGIN PUBLIC KEY-----\nMIIEFDCCAvygAwIBAgIUean2nHOAJYkSP9rjE/iHpo+f4I4wDQYJKoZIhvcNAQEL\nBQAwgcMxDjAMBgNVBAoMBVJFTU1FMVIwUAYKCZImiZPyLGQBAQxCMDIzZTU4NDEy\nOTYzNDRiNzIzNzJlZWYyNGQ2N2M4YmQ0YjI4MTIxNjcyY2Q2MzkyZTAxZmEyNWQ0\nMDU5MGI5YmVjMQswCQYDVQQGEwJVUzESMBAGA1UEAwwJdXNlck5hbWUxMQ0wCwYD\nVQQqDARKb2huMQ4wDAYDVQQEDAVTbWl0aDEdMBsGCSqGSIb3DQEJARYOdXNlckBl\nbWFpbC5jb20wHhcNMTgwNTIzMTQ0ODI5WhcNMTkwNTE4MTQ0ODI5WjCBwzEOMAwG\nA1UECgwFUkVNTUUxUjBQBgoJkiaJk/IsZAEBDEIwMjNlNTg0MTI5NjM0NGI3MjM3\nMmVlZjI0ZDY3YzhiZDRiMjgxMjE2NzJjZDYzOTJlMDFmYTI1ZDQwNTkwYjliZWMx\nCzAJBgNVBAYTAlVTMRIwEAYDVQQDDAl1c2VyTmFtZTExDTALBgNVBCoMBEpvaG4x\nDjAMBgNVBAQMBVNtaXRoMR0wGwYJKoZIhvcNAQkBFg51c2VyQGVtYWlsLmNvbTCC\nASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKYpxzdOjvIvjfPnnYCFB0M3\nfflJQuxt+OGdKODumkKdajjuFoTVM44KqFdHoG5z46uDCo9ggDFY6VCnEOVelV8B\n/4IabU/k+Y9b/qqBYuTOhiemEXQwN+ZNVWy2XFGej2bOvphLZDKTfevBJq91p4ld\nR61bEJgUzZBraj2gM5XKnNmCc1TmUDsOdCcTpSZWLSfaeCNuTNk0ncO3kbT8STGL\nu5flhm1iL1IB6AUULyyyKTlcH9yVcvUn5Y+8YNMlKo/biM1A7iodGWN7bzGt3CEu\nT2kUpaNaJmF/3XnXDEn1KBp5MSiMUjNV7LDSKxDY79ggZQl0pIn1I3lbuvSyz5EC\nAwEAATANBgkqhkiG9w0BAQsFAAOCAQEAgj/ds9cgkd/u7IOtBPJ3d4ZRxcvl7sjN\n1prl4j5l2afa8GqRwFB2iYggiBcP6k33C28FH1414WmtXDZU5nnKka5EOUJNZx9K\nTUStBTfjaj+yYm1bm5jPHcveKQJbmwjGGnCH+xxp2agtZVID2UGm1mSGtIfId09D\nEHezzBnK1vZCIvNT2H0I3NpBvcs0i6CWAqEGIP5aItpAxEcE3wYteFMDzX9C4sy6\nC0soLmSG+oOXfEG58y/18RLsj+jpqELGmHPEA72c/iuEwe4YQnNbhUWAiTS+GLjz\nvLFCgyKWf8jWQNYpPuV2xMwMItbrB5NNVro3rnLe9z48l7aN83xFtQ==\n-----END PUBLIC KEY-----\n";
        }

        #endregion
    }
}
