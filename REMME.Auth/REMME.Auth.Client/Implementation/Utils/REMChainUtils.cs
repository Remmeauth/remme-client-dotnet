using System;
using System.Collections.Generic;
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

        public static string GetSettingsAddressFromData(string data)
        {
            var dataParts = data.Split('.').Take(4).ToArray();
            var addressParts = new List<string>();
            foreach (var dataPart in dataParts)
            {
                addressParts.Add(new string(dataPart.Sha256Digest().BytesToHexString().Take(16).ToArray()));
            }
            while(addressParts.Count != 4)
            {
                addressParts.Add(new string("".Sha256Digest().BytesToHexString().Take(16).ToArray()));
            }

            return string.Format("000000{0}", String.Join(String.Empty, addressParts.ToArray()));
        }
    }
}
