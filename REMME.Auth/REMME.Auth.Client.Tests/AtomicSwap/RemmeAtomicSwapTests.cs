using Moq;
using NUnit.Framework;
using REMME.Auth.Client.Contracts;
using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.Implementation;
using REMME.Auth.Client.RemmeApi;
using REMME.Auth.Client.RemmeApi.Models.AtomicSwap;
using REMME.Auth.Client.RemmeApi.Models.Proto;
using System;

namespace REMME.Auth.Client.Tests.AtomicSwap
{
    [TestFixture]
    public class RemmeAtomicSwapTests
    {
        private const string MOCK_BATCH_ID = "Fake BATCH ID";
        private const string MOCK_SWAP_ID = "033102e41346242476b15a3a7966eb5249271025fc7fb0b37ed3fdb4bcce6892";

        [Test]
        public void Init_ValidDataProvided_TransactionSent()
        {
            // Arrange
            var mockRest = new Mock<IRemmeRest>();
            var mockTransactionService = new Mock<IRemmeTransactionService>();
            mockTransactionService.Setup(a => a.SendTransaction(It.IsAny<Transaction>()))
                    .ReturnsAsync(new BaseTransactionResponse(string.Empty) { BatchId = MOCK_BATCH_ID });

            var swap = new RemmeAtomicSwap(mockRest.Object, mockTransactionService.Object);

            //Act
            var actualCheckResult = swap.Init(GetValidInitMock()).Result;

            //Assert
            Assert.AreEqual(actualCheckResult.BatchId, MOCK_BATCH_ID, "BATCH_ID must be returned");
        }

        [Test]
        public void Init_InValidDataProvided_ExceptionThrown()
        {
            // Arrange
            var mockTransactionService = new Mock<IRemmeTransactionService>();
            var mockRest = new Mock<IRemmeRest>();

            var mock = new Mock<IRemmeTransactionService>();
            mock.Setup(a => a.SendTransaction(It.IsAny<Transaction>()))
                    .ReturnsAsync(new BaseTransactionResponse(string.Empty) { BatchId = MOCK_BATCH_ID });

            var swap = new RemmeAtomicSwap(mockRest.Object, mockTransactionService.Object);
            
            //Assert
            Assert.That(() => swap.Init(GetInValidInitMock()),
                        Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void GeInfo_SwapIdProvided_InfoReturned()
        {
            // Arrange
            var mockTransactionService = new Mock<IRemmeTransactionService>();
            var mockRest = new Mock<IRemmeRest>();

            mockRest.Setup(a => a.GetRequest<SwapInfoDto>(RemmeMethodsEnum.AtomicSwapInfo, It.IsAny<string>()))
                    .ReturnsAsync(new SwapInfoDto() { SwapId = MOCK_SWAP_ID });
            var swap = new RemmeAtomicSwap(mockRest.Object, mockTransactionService.Object);

            //Act
            var actualCheckResult = swap.GetInfo(MOCK_SWAP_ID).Result;

            //Assert
            Assert.AreEqual(actualCheckResult.SwapId, MOCK_SWAP_ID, "Valid SWAP_ID must be returned");
        }

        private SwapInitDto GetValidInitMock()
        {
            return new SwapInitDto
            {
                Amount = 100,
                CreatedAt = DateTime.UtcNow,
                EmailAddressEncryptedByInitiator = "0x656d61696c",
                ReceiverAddress = "112007484def48e1c6b77cf784aeabcac51222e48ae14f3821697f4040247ba01558b1",
                SecretLockBySolicitor = "aa273f38cf1d9c0fda0ee67b08927278b368db5927e4bf4c0aac15b95ac12df6",
                SenderAddress = "0xe6ca0e7c974f06471759e9a05d18b538c5ced11e",
                SwapId = "033102e41346242476b15a3a7966eb5249271025fc7fb0b37ed3fdb4bcce6892"
            };
        }

        private SwapInitDto GetInValidInitMock()
        {
            return new SwapInitDto
            {
                Amount = 100,
                CreatedAt = DateTime.UtcNow,
                EmailAddressEncryptedByInitiator = "testEmailEncr",
                ReceiverAddress = "receiverAddress",
                SecretLockBySolicitor = "lockTest",
                SenderAddress = "testAddress",
                SwapId = "testId"
            };
        }
    }
}