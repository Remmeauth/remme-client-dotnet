using Moq;
using NUnit.Framework;
using REMME.Auth.Client.Contracts.Exceptions;
using REMME.Auth.Client.Implementation;
using REMME.Auth.Client.RemmeApi;
using REMME.Auth.Client.RemmeApi.Models.Token;
using System;

namespace REMME.Auth.Client.Tests.Token
{
    [TestFixture]
    public class RemmeTokenTests
    {
        private const int MOCK_BALANCE = 123;
        private const string MOCK_PUBLIC_KEY = "somePublicKey";
        private const string MOCK_EXCEPTION_MESSAGE = "Remme Node exception Message";
        private const string MOCK_BATCH_ID = "Fake BATCH ID";

        [Test]
        public void GetBalance_ValidKeyProvided_BalanceReturned()
        {

            // Arrange
            var mock = new Mock<IRemmeRest>();
            mock.Setup(a => a.GetRequest<BalanceCheckResult>
                (It.Is<RemmeMethodsEnum>(t => RemmeMethodsEnum.Token == t), It.IsAny<string>()))
                .ReturnsAsync(new BalanceCheckResult() { Balance = MOCK_BALANCE.ToString() });

            var token = new RemmeToken(mock.Object);

            //Act
            var actualBalance = token.GetBalance(MOCK_PUBLIC_KEY).Result;

            //Assert
            Assert.AreEqual(MOCK_BALANCE, actualBalance, "Balance should be returned");
        }

        [Test]
        public void GetBalance_RemmeRestException_ExceptionThrown()
        {

            // Arrange
            var mock = new Mock<IRemmeRest>();
            mock.Setup(a => a.GetRequest<BalanceCheckResult>
                (It.Is<RemmeMethodsEnum>(t => RemmeMethodsEnum.Token == t), It.IsAny<string>()))
                .Throws(new RemmeNodeException(MOCK_EXCEPTION_MESSAGE));

            var token = new RemmeToken(mock.Object);

            // Assert
            Assert.That(() => token.GetBalance(MOCK_PUBLIC_KEY),
                        Throws.TypeOf<RemmeNodeException>());
        }


        [Test]
        public void GetBalance_RemmeConnectionException_ExceptionThrown()
        {

            // Arrange
            var mock = new Mock<IRemmeRest>();
            mock.Setup(a => a.GetRequest<BalanceCheckResult>
                (It.Is<RemmeMethodsEnum>(t => RemmeMethodsEnum.Token == t), It.IsAny<string>()))
                .Throws(new RemmeConnectionException(MOCK_EXCEPTION_MESSAGE));

            var token = new RemmeToken(mock.Object);

            // Assert
            Assert.That(() => token.GetBalance(MOCK_PUBLIC_KEY),
                        Throws.TypeOf<RemmeConnectionException>());
        }

        [Test]
        public void Transfer_ValidKeyProvided_BatchIdReturned()
        {

            // Arrange
            var mock = new Mock<IRemmeRest>();
            mock.Setup(a => a.PostRequest<TransferPayload, TransferResult>
                             (It.Is<RemmeMethodsEnum>(t => RemmeMethodsEnum.Token == t), It.IsAny<TransferPayload>()))
                             .ReturnsAsync(new TransferResult() {  BachId = MOCK_BATCH_ID});

            var token = new RemmeToken(mock.Object);
            //Act
            var actualTransactionResult = token.Transfer(MOCK_PUBLIC_KEY, MOCK_BALANCE).Result;

            //Assert
            Assert.AreEqual(actualTransactionResult.BatchId, MOCK_BATCH_ID, "Transaction result object should contain valid batch id");            
        }

        [Test]
        public void Transfer_RemmeRestException_ExceptionThrown()
        {

            // Arrange
            var mock = new Mock<IRemmeRest>();
            mock.Setup(a => a.PostRequest<TransferPayload, TransferResult>
                             (It.Is<RemmeMethodsEnum>(t => RemmeMethodsEnum.Token == t), It.IsAny<TransferPayload>()))
                .Throws(new RemmeNodeException(MOCK_EXCEPTION_MESSAGE));

            var token = new RemmeToken(mock.Object);

            // Assert
            Assert.That(() => token.Transfer(MOCK_PUBLIC_KEY, MOCK_BALANCE),
                        Throws.TypeOf<RemmeNodeException>());
        }


        [Test]
        public void Transfer_RemmeConnectionException_ExceptionThrown()
        {

            // Arrange
            var mock = new Mock<IRemmeRest>();
            mock.Setup(a => a.PostRequest<TransferPayload, TransferResult>
                             (It.Is<RemmeMethodsEnum>(t => RemmeMethodsEnum.Token == t), It.IsAny<TransferPayload>()))
                .Throws(new RemmeConnectionException(MOCK_EXCEPTION_MESSAGE));

            var token = new RemmeToken(mock.Object);

            // Assert
            Assert.That(() => token.Transfer(MOCK_PUBLIC_KEY, MOCK_BALANCE),
                        Throws.TypeOf<RemmeConnectionException>());
        }
    }
}
