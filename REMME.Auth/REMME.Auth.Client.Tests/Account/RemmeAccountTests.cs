using NUnit.Framework;
using REMME.Auth.Client.Contracts.Exceptions;
using REMME.Auth.Client.Implementation;

namespace REMME.Auth.Client.Tests.Account
{
    [TestFixture]
    public class RemmeAccountTests
    {
        private const string MOCK_PRIVATE_KEY = "cc47faaca8194a63cd402bfe143c6105f394f6cc12a4db68e8a9e84941396926";
        private const string MOCK_PUBLIC_KEY = "0227ab6d4fd61b8b69ddd7a4f69f5287287ed4171a58af4a7d2816cfe614ef91da";
        private const string MOCK_DATA_HEX = "0a423032336535383431323936333434623732333732656566323464363763386264346232383132313637326364363339326530316661323564343035393062396265631a076163636f756e742203302e31320f3844354337323336334541373943344a800133623936303066373165663437656562323862303361373564663832626639356137376439316632613166313838383366656530313237303239626139343564313963383762376364323737623533663636613565316435383265633964323638656565366430306539366261613266393863313961393533303235313062655242303232376162366434666436316238623639646464376134663639663532383732383765643431373161353861663461376432383136636665363134656639316461";
        private const string MOCK_REMME_ADDRESS = "112007407df0c56020ec21f3655fed9c64ce9586ba4a8b190c511d7452fb7ad642421e";
        private const string MOCK_DATA_SIGNATURE = "948bc2d0849b8d4d72347e9b232bdea78268fa5d8e8f2986605f42c5a87e0ac20241a1506b9f5ca4c1e8c839e5fb4df693453d718322c51309cd540605917155";

        [Test]
        public void CreateAccount_NoParamsPassed_NewKeyPairCreated()
        {
            ////Act
            var actualAccount = new RemmeAccount();

            ////Assert            
            Assert.IsFalse(string.IsNullOrEmpty(actualAccount.PublicKeyHex), "PublicKeyHex should be initialized");
            Assert.IsFalse(string.IsNullOrEmpty(actualAccount.PrivateKeyHex), "PrivateKeyHex should be initialized");
            Assert.IsFalse(string.IsNullOrEmpty(actualAccount.Address), "Address should be initialized");
        }


        [Test]
        public void CreateAccount_PrivateKeyPassed_ValidPublicKeyCreated()
        {
            ////Arange 
            var privateHex = MOCK_PRIVATE_KEY;
            var expectedPublic = MOCK_PUBLIC_KEY;

            ////Act
            var actualPublicKey = new RemmeAccount(privateHex).PublicKeyHex;

            ////Assert            
            Assert.AreEqual(expectedPublic, actualPublicKey, "PublicKeyHex should be calculated from private");
        }

        [Test]
        public void CreateAccount_PrivateKeyPassed_ValidRemmeAddressCreated()
        {
            ////Arange 
            var privateHex = MOCK_PRIVATE_KEY;
            var expectedAddress = MOCK_REMME_ADDRESS;

            ////Act
            var actualAddress = new RemmeAccount(privateHex).Address;

            ////Assert            
            Assert.AreEqual(expectedAddress, actualAddress, "REMChain Address should be calculated from public key");
        }

        [Test]
        public void Sign_ValidPrivateKeyPassed_ValidRemmeAddressCreated()
        {
            ////Arange 
            var privateHex = MOCK_PRIVATE_KEY;
            var expectedSignature = MOCK_DATA_SIGNATURE;
            var hexData = MOCK_DATA_HEX;

            var account = new RemmeAccount(privateHex);

            ////Act
            var actualSignature = account.Sign(hexData);

            ////Assert            
            Assert.AreEqual(expectedSignature, actualSignature, "Data should be signed properly");
        }

        [Test]
        public void Sign_InValidHexDataPassed_ValidRemmeAddressCreated()
        {
            ////Arange 
            var privateHex = MOCK_PRIVATE_KEY;
            var hexData = "Invalid Hex";

            var account = new RemmeAccount(privateHex);
            
            ////Assert 
            Assert.That(() => account.Sign(hexData),
                        Throws.TypeOf<InvalidHexFormat>());
        }
    }
}
