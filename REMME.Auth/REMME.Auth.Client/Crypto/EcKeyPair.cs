using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using REMME.Auth.Client.Implementation.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace REMME.Auth.Client.Crypto
{
    /// <summary>
    /// Represents an Elliptic Curve Keypair.
    /// </summary>
    public class EcKeyPair
    {
        public readonly BigInteger _privateKey;

        public byte[] PrivateKey { get => PrivateKeyToBytes(); }
        public byte[] PublicKey { get; private set; }

        /// <summary>
        /// Generates an new Elliptic Curve Keypair.
        /// </summary>
        private EcKeyPair(bool compressedPublicKey = true)
        {
            var generator = new ECKeyPairGenerator();
            var keygenParams = new ECKeyGenerationParameters(_ecParams, _secureRandom);
            generator.Init(keygenParams);

            var keypair = generator.GenerateKeyPair();

            var privParams = keypair.Private as ECPrivateKeyParameters;
            var pubParams = keypair.Public as ECPublicKeyParameters;

            _privateKey = privParams.D;
            PublicKey = GetPublicKey(_privateKey, compressedPublicKey);
        }

        /// <summary>
        /// Creates an Elliptic Curve Keypair from the private key.
        /// </summary>
        /// <param name="privKey">PrivateKeyHex key bytes</param>        
        public EcKeyPair(byte[] privateKey, bool compressedPublicKey = true)
        {
            _privateKey = new BigInteger(1, privateKey);
            PublicKey = GetPublicKey(_privateKey, compressedPublicKey);
        }

        /// <summary>
        /// Creates an Elliptic Curve Keypair from the private key.
        /// </summary>
        /// <param name="privKey">PrivateKeyHex key string</param>        
        public EcKeyPair(string privateKeyHex, bool compressedPublicKey = true)
        {
            _privateKey = new BigInteger(1, privateKeyHex.HexStringToBytes());
            PublicKey = GetPublicKey(_privateKey, compressedPublicKey);
        }

        /// <summary>
        /// Gets public key bytes form private key bytes
        /// </summary>
        /// <param name="privKey"></param>
        /// <param name="compressedPublicKey"></param>
        /// <returns></returns>
        public byte[] GetPublicKey(BigInteger privKey, bool compressedPublicKey)
        {
            if (compressedPublicKey)
            {
                int Y = _ecParams.G.Multiply(_privateKey)
                                    .Normalize()
                                    .YCoord
                                    .ToBigInteger().IntValue;

                var result = _ecParams.G.Multiply(_privateKey).GetEncoded().Take(33).ToArray();
                result[0] = Convert.ToByte(Y % 2 == 0 ? 2 : 3);
                return result;
            }

            return _ecParams.G.Multiply(_privateKey).GetEncoded();
        }

        /// <summary>
        /// Calculates ECDSA signature for  given input
        /// </summary>
        public byte[] Sign(byte[] input)
        {
            var signer = new DeterministicECDSA();
            var privKey = new ECPrivateKeyParameters(_privateKey, _ecParams);

            signer.SetPrivateKey(privKey);
            var sig = ECDSASignature.FromDER(signer.Sign(input)).ToCanonical();
           
            var r_arr = sig.R.ToByteArrayUnsigned();
            var s_arr = sig.S.ToByteArrayUnsigned();
            
            var rsigPad = new byte[32];
            Array.Copy(r_arr, 0, rsigPad, rsigPad.Length - r_arr.Length, r_arr.Length);

            var ssigPad = new byte[32];
            Array.Copy(s_arr, 0, ssigPad, ssigPad.Length - s_arr.Length, s_arr.Length);
                
            return MergeByteArrays(rsigPad, ssigPad);
        }

        #region Helpers

        private byte[] PrivateKeyToBytes()
        {
            return _privateKey.ToByteArrayUnsigned();
        }

        private static byte[] MergeByteArrays(params byte[][] arrays)
        {
            return MergeToEnum(arrays).ToArray();
        }
        
        private static IEnumerable<byte> MergeToEnum(params byte[][] arrays)
        {
            foreach (var a in arrays)
            foreach (var b in a)
                yield return b;
        }
        
        #endregion

        #region Static Members

        private static string CURVE_NAME = "secp256k1";
        private static readonly ECDomainParameters _ecParams;
        private static readonly SecureRandom _secureRandom;

        public static readonly BigInteger CurveOrder;
        public static readonly BigInteger HalfCurveOrder;

        static EcKeyPair()
        {
            var secp256k1 = SecNamedCurves.GetByName(CURVE_NAME);
            _ecParams = new ECDomainParameters(secp256k1.Curve,
                                               secp256k1.G,
                                               secp256k1.N,
                                               secp256k1.H);

            CurveOrder = secp256k1.N;
            HalfCurveOrder = secp256k1.N.ShiftRight(1);

            _secureRandom = new SecureRandom();
        }

        public static EcKeyPair GenerateNewKeyPair()
        {
            var keyPair = new EcKeyPair();

            if (keyPair.PrivateKey.Length != 32)
                return GenerateNewKeyPair();

            return keyPair;
        }
        #endregion
    }
}
