using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace REMME.Auth.Client.Implementation.Utils
{
    public static class CertificateExtensions
    {
        public const string CERT_PEM_FORMAT = "-----BEGIN CERTIFICATE-----\n{0}\n-----END CERTIFICATE-----";

        public static string ToPemString(this X509Certificate2 cert)
        {
            return string.Format(CERT_PEM_FORMAT, Convert.ToBase64String(cert.Export(X509ContentType.Cert)));
        }

        public static string GetPemPublicKey(this X509Certificate2 cert)
        {             
            return string.Format(RSAExtensions.PUBLIC_KEY_PEM_FORMAT, Convert.ToBase64String(GetPublicKeyBytes(cert)));
        }

        public static byte[] GetPublicKeyBytes(this X509Certificate2 cert)
        {
            return BuildPublicKeyPem(cert);
        }

        public static X509Certificate2 SystemX509FromPem(this string pemCertificate)
        {
            var pemString = pemCertificate
                             .Replace("-----BEGIN CERTIFICATE-----", "")
                             .Replace("-----END CERTIFICATE-----", "");
            return new X509Certificate2(Convert.FromBase64String(pemString));
        }

        #region Helpers 

        private static byte[] BuildPublicKeyPem(X509Certificate2 cert)
        {
            byte[] algOid;

            switch (cert.GetKeyAlgorithm())
            {
                case "1.2.840.113549.1.1.1":
                    algOid = new byte[] { 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01 };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cert), $"Need an OID lookup for {cert.GetKeyAlgorithm()}");
            }

            byte[] algParams = cert.GetKeyAlgorithmParameters();
            byte[] publicKey = WrapAsBitString(cert.GetPublicKey());

            byte[] algId = BuildSimpleDerSequence(algOid, algParams);
            byte[] spki = BuildSimpleDerSequence(algId, publicKey);

            return spki;
        }

        private static byte[] WrapAsBitString(byte[] value)
        {
            byte[] len = EncodeDerLength(value.Length + 1);
            byte[] bitString = new byte[value.Length + len.Length + 2];
            bitString[0] = 0x03;
            Buffer.BlockCopy(len, 0, bitString, 1, len.Length);
            bitString[len.Length + 1] = 0x00;
            Buffer.BlockCopy(value, 0, bitString, len.Length + 2, value.Length);
            return bitString;
        }

        private static byte[] BuildSimpleDerSequence(params byte[][] values)
        {
            int totalLength = values.Sum(v => v.Length);
            byte[] len = EncodeDerLength(totalLength);
            int offset = 1;

            byte[] seq = new byte[totalLength + len.Length + 1];
            seq[0] = 0x30;

            Buffer.BlockCopy(len, 0, seq, offset, len.Length);
            offset += len.Length;

            foreach (byte[] value in values)
            {
                Buffer.BlockCopy(value, 0, seq, offset, value.Length);
                offset += value.Length;
            }

            return seq;
        }

        private static byte[] EncodeDerLength(int length)
        {
            if (length <= 0x7F)
            {
                return new byte[] { (byte)length };
            }

            if (length <= 0xFF)
            {
                return new byte[] { 0x81, (byte)length };
            }

            if (length <= 0xFFFF)
            {
                return new byte[]
                {
            0x82,
            (byte)(length >> 8),
            (byte)length,
                };
            }

            if (length <= 0xFFFFFF)
            {
                return new byte[]
                {
                    0x83,
                    (byte)(length >> 16),
                    (byte)(length >> 8),
                    (byte)length,
                };
            }

            return new byte[]
            {
                0x84,
                (byte)(length >> 24),
                (byte)(length >> 16),
                (byte)(length >> 8),
                (byte)length,
            };
        }

        #endregion
    }
}
