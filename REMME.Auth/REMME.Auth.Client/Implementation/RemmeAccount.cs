using REMME.Auth.Client.Contracts;
using REMME.Auth.Client.Crypto;
using REMME.Auth.Client.Implementation.Utils;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeAccount : IRemmeAccount
    {
        private EcKeyPair _keyPair;
        public const string ACCOUNT_FAMILY_NAME = "account";

        #region Constructors

        public RemmeAccount(string privateKeyHex)
            : this(new EcKeyPair(privateKeyHex)) { }

        public RemmeAccount(byte[] privateKeyBytes)
            : this(new EcKeyPair(privateKeyBytes)) { }

        public RemmeAccount()
            : this(EcKeyPair.GenerateNewKeyPair()) { }

        public RemmeAccount(EcKeyPair keyPair)
        {
            _keyPair = keyPair;
        }

        #endregion

        public string Address { get => REMChainUtils.GetAddressFromData(PublicKeyHex, ACCOUNT_FAMILY_NAME); }

        public string PrivateKeyHex { get => _keyPair.PrivateKey.BytesToHexString(); }

        public string PublicKeyHex { get => _keyPair.PublicKey.BytesToHexString(); }

        public string Sign(string hexData)
        {
            return Sign(hexData.HexStringToBytes());
        }

        public string Sign(byte[] data)
        {
            return _keyPair.Sign(data).BytesToHexString();
        }
    }
}
