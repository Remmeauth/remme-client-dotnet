namespace REMME.Auth.Client.Contracts
{
    /// <summary>
    /// Helper for managing REMChain account and data signing
    /// </summary>
    public interface IRemmeAccount
    {
        /// <summary>
        /// Represents Address of an Account on REMChain
        /// </summary>
        string Address { get; }

        /// <summary>
        /// Gets Private Key Bytes in HEX
        /// </summary>
        string PrivateKeyHex { get; }

        /// <summary>
        /// Gets Public Key Bytes in HEX
        /// </summary>
        string PublicKeyHex { get; }

        /// <summary>
        /// Signes data using account Private Key
        /// </summary>
        /// <param name="hexData">Data wich should be signed in HEX</param>
        /// <returns>HEX string representing data signature</returns>
        string Sign(string hexData);

        /// <summary>
        /// Signes data using account Private Key
        /// </summary>
        /// <param name="data">Data bytes wich should be signed</param>
        /// <returns>HEX string representing data signature</returns>
        string Sign(byte[] data);
    }
}
