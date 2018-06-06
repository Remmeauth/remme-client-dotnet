using Google.Protobuf;
using System.Collections.Generic;

namespace REMME.Auth.Client.Contracts.Models
{
    public class TransactionCreateDto
    {
        public ByteString Payload { get; set; }

        public List<string> Inputs { get; set; }

        public List<string> Outputs { get; set; }

        public string FamilyName { get; set; }

        public string FamilyVersion { get; set; }
    }
}
