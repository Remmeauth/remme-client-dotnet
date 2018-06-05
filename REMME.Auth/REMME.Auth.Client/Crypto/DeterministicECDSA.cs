using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace REMME.Auth.Client.Crypto
{
    public class DeterministicECDSA : ECDsaSigner
    {
        private readonly IDigest _digest;

        public DeterministicECDSA()
            : base(new HMacDsaKCalculator(new Sha256Digest()))
        {
            _digest = new Sha256Digest();
        }

        public void SetPrivateKey(ECPrivateKeyParameters ecKey)
        {
            Init(true, ecKey);
        }

        public byte[] Sign(byte[] data)
        {
            var hash = new byte[_digest.GetDigestSize()];

            _digest.BlockUpdate(data, 0, data.Length);
            _digest.DoFinal(hash, 0);
            _digest.Reset();

            return SignHash(hash);
        }

        public byte[] SignHash(byte[] hash)
        {
            return new ECDSASignature(this.GenerateSignature(hash)).ToDER();
        }
    }
}
