using Org.BouncyCastle.Crypto.Digests;
using REMME.Auth.Client.Contracts.Exceptions;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace REMME.Auth.Client.Crypto
{
    public static class ByteExtensions
    {
        public static byte[] HexStringToBytes(this string hexString)
        {
            byte[] result;
            try
            {
                result = Enumerable
                               .Range(0, hexString.Length)
                               .Where(x => x % 2 == 0)
                               .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16)).ToArray();
            }
            catch (FormatException ex)
            {
                throw new InvalidHexFormat(ex.Message);
            }

            return result;
        }

        public static string BytesToHexString(this byte[] bytes)
        {
            return string.Concat(bytes.Select(byteb => byteb.ToString("x2")).ToArray());
        }

        /// <summary>
        /// Calculates the SHA512 64 byte checksum of the input string
        /// </summary>
        /// <param name="input">Input string to get checksum</param>
        /// <returns>64 byte array checksum</returns>
        public static byte[] Sha512Digest(this string input)
        {
            using (SHA512 shaM = new SHA512Managed())
            {
                var data = Encoding.UTF8.GetBytes(input);
                return shaM.ComputeHash(data);
            }
        }

        /// <summary>
        /// Calculates the SHA512 64 byte checksum of the input bytes
        /// </summary>
        /// <param name="input">Bytes input to get checksum</param>
        /// <returns>64 byte array checksum</returns>
        public static byte[] Sha512Digest(this byte[] input)
        {
            var algorithm = new Sha512Digest();
            Byte[] firstHash = new Byte[algorithm.GetDigestSize()];
            algorithm.BlockUpdate(input, 0, input.Length);
            algorithm.DoFinal(firstHash, 0);
            return firstHash;
        }
    }
}
