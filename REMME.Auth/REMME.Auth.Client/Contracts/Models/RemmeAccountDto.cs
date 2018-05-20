namespace REMME.Auth.Client.Contracts.Models
{
    public class RemmeAccountDto
    {
        public string RemChainAdress { get; set; }

        //TODO: Will be modified to be secp256k1 pair while implementing REM-193
        public string PrivateKey { get; set; }
    }
}
