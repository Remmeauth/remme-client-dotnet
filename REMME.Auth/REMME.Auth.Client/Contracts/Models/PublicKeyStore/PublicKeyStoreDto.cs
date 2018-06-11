namespace REMME.Auth.Client.Contracts.Models.PublicKeyStore
{
    public class PublicKeyStoreDto
    {
        public string PublicKeyPem { get; set; }

        public PublicKeyTypeEnum PublicKeyType { get; set; }

        public EntityOwnerTypeEnum EntityOwnerType { get; set; }

        public int RsaKeySize { get; set; }

        public long RsaPublicExponent { get; set; }

        public string EntityHash { get; set; }
        public string EntityHashSignature { get; set; }
    
        public uint ValidityFrom { get; set; }
        public uint ValidityTo { get; set; }
    }
}
