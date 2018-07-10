using System;

namespace REMME.Auth.Client.RemmeApi.Models.AtomicSwap
{
    public class SwapInitDto
    {
        public string ReceiverAddress { get; set; }

        public string SenderAddress { get; set; }

        public uint Amount { get; set; }

        public string SwapId { get; set; }

        public string SecretLockBySolicitor { get; set; }

        public string EmailAddressEncryptedByInitiator { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
