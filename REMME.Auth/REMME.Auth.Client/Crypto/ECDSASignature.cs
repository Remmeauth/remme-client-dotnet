using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using System;
using System.IO;

namespace REMME.Auth.Client.Crypto
{
    public class ECDSASignature
    {
        private const string FORMAT_EXCEPTION_MESSAGE = "Invalid signature format in DER";

        public BigInteger R { get; }

        public BigInteger S { get; }

        public byte[] V { get; set; }

        public ECDSASignature(BigInteger r, BigInteger s)
        {
            R = r;
            S = s;
        }

        public ECDSASignature(BigInteger[] rs)
        {
            R = rs[0];
            S = rs[1];
        }

        public ECDSASignature(byte[] derSig)
        {
            try
            {
                var decoder = new Asn1InputStream(derSig);
                var seq = decoder.ReadObject() as DerSequence;

                if (seq == null || seq.Count != 2)
                    throw new FormatException(FORMAT_EXCEPTION_MESSAGE);

                R = (seq[0] as DerInteger).Value;
                S = (seq[1] as DerInteger).Value;
            }
            catch (Exception ex)
            {
                throw new FormatException(FORMAT_EXCEPTION_MESSAGE, ex);
            }
        }

        #region Helpers

        public static ECDSASignature FromDER(byte[] sig)
        {
            return new ECDSASignature(sig);
        }

        public byte[] ToDER()
        {
            var derSignature = new MemoryStream(64);
            var sequence = new DerSequenceGenerator(derSignature);

            sequence.AddObject(new DerInteger(R));
            sequence.AddObject(new DerInteger(S));
            sequence.Close();

            return derSignature.ToArray();
        }

        #endregion
    }

}
