using System;
using System.Linq;

namespace REMME.Auth.Client.Implementation.Utils
{
    public class REMChainUtils
    {
        public static string GetAddressFromData(string data, string familyName)
        {
            var hashData = data.Sha512Digest()
                                .BytesToHexString()
                                .Take(64).ToArray();
            var hashFamily = familyName
                                .Sha512Digest()
                                .BytesToHexString()
                                .Take(6).ToArray();

            return string.Format("{0}{1}", new String(hashFamily), new String(hashData));
        }
    }
}
