using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;
using System;
using System.IO;
using System.Security.Cryptography;

namespace REMME.Auth.Client.Implementation.Utils
{
    public static class RSAExtensions
    {
        public const string PUBLIC_KEY_PEM_FORMAT = "-----BEGIN PUBLIC KEY-----\n{0}\n-----END PUBLIC KEY-----";
        public const string PRIVATE_RSA_KEY_PEM_FORMAT = "-----BEGIN RSA PRIVATE KEY-----\n{0}\n-----END RSA PRIVATE KEY-----";

        public static byte[] GetKeyBytes(this RSAParameters rsaParameters)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30);
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 });
                    EncodeIntegerBigEndian(innerWriter, rsaParameters.Modulus);
                    EncodeIntegerBigEndian(innerWriter, rsaParameters.Exponent);
                    EncodeIntegerBigEndian(innerWriter, rsaParameters.D);
                    EncodeIntegerBigEndian(innerWriter, rsaParameters.P);
                    EncodeIntegerBigEndian(innerWriter, rsaParameters.Q);
                    EncodeIntegerBigEndian(innerWriter, rsaParameters.DP);
                    EncodeIntegerBigEndian(innerWriter, rsaParameters.DQ);
                    EncodeIntegerBigEndian(innerWriter, rsaParameters.InverseQ);
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }
                return stream.GetBuffer();
            }
        }

        public static RSAParameters GetPrivateRsaParameters(this AsymmetricCipherKeyPair keyPair)
        {
            var keyParams = (RsaPrivateCrtKeyParameters)keyPair.Private;
            RSAParameters rsaParameters = new RSAParameters();

            rsaParameters.Modulus = keyParams.Modulus.ToByteArrayUnsigned();
            rsaParameters.P = keyParams.P.ToByteArrayUnsigned();
            rsaParameters.Q = keyParams.Q.ToByteArrayUnsigned();
            rsaParameters.DP = keyParams.DP.ToByteArrayUnsigned();
            rsaParameters.DQ = keyParams.DQ.ToByteArrayUnsigned();
            rsaParameters.InverseQ = keyParams.QInv.ToByteArrayUnsigned();
            rsaParameters.D = keyParams.Exponent.ToByteArrayUnsigned();
            rsaParameters.Exponent = keyParams.PublicExponent.ToByteArrayUnsigned();

            return rsaParameters;
        }

        public static RSACryptoServiceProvider GetCryptoServiceProvider(this AsymmetricCipherKeyPair keyPair)
        {
            var rsaKey = new RSACryptoServiceProvider();
            rsaKey.ImportParameters(GetPrivateRsaParameters(keyPair));

            return rsaKey;
        }

        public static byte[] GetPublicKeyBytes(this AsymmetricCipherKeyPair keyPair)
        {
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);
            return publicKeyInfo.ToAsn1Object().GetDerEncoded();
        }

        public static byte[] GetPrivateKeyBytes(this AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.GetPrivateRsaParameters().GetKeyBytes();
        }

        public static string GetPublicKeyPem(this AsymmetricCipherKeyPair keyPair)
        {
            return string.Format(PUBLIC_KEY_PEM_FORMAT, Convert.ToBase64String(keyPair.GetPublicKeyBytes()));
        }

        public static string GetPrivateKeyPem(this AsymmetricCipherKeyPair keyPair)
        {
            return string.Format(PRIVATE_RSA_KEY_PEM_FORMAT, Convert.ToBase64String(keyPair.GetPrivateKeyBytes()));
        }

        public static bool IsValidKeyParams(this AsymmetricCipherKeyPair keyPair, int keySize)
        {
            var devideByEight = Math.Ceiling(keySize / 8.0);
            var devideBySixteen = Math.Ceiling(keySize / 16.0);

            var keyParams = keyPair.GetPrivateRsaParameters();
            return devideByEight == keyParams.D.Length
                && devideBySixteen == keyParams.DP.Length
                && devideBySixteen == keyParams.DQ.Length
                && 3 == keyParams.Exponent.Length
                && devideBySixteen == keyParams.InverseQ.Length
                && devideByEight == keyParams.Modulus.Length
                && devideBySixteen == keyParams.P.Length
                && devideBySixteen == keyParams.Q.Length;
        }

        #region Helpers

        private static void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
            if (length < 0x80)
            {
                stream.Write((byte)length);
            }
            else
            {
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }
        private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02);
            var prefixZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }

        #endregion
    }
}
