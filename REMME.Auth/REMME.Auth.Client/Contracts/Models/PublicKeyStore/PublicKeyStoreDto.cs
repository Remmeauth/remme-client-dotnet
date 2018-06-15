using Org.BouncyCastle.Crypto;

namespace REMME.Auth.Client.Contracts.Models.PublicKeyStore
{
    public class PublicKeyStoreDto
    {
        public AsymmetricCipherKeyPair KeyPair { get; set; }

        public string EntityData { get; set; }

        public EntityOwnerTypeEnum EntityOwnerType { get; set; }
        public PublicKeyTypeEnum PublicKeyType { get; set; }

        public uint ValidityFrom { get; set; }
        public uint ValidityTo { get; set; }
    }
}
