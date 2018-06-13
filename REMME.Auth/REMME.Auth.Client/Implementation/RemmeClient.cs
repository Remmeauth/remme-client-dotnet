using REMME.Auth.Client.Contracts;
using REMME.Auth.Client.RemmeApi;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeClient : IRemmeClient
    {
        private readonly IRemmeRest _remmeRest;
        private readonly IRemmeTransactionService _remmeTransactionService;

        /// <summary>
        /// Initialize new instance of RemmeClient
        /// </summary>
        /// <param name="nodeAddress">Address and port of the REMME node Rest API</param>
        /// <param name="socketAddress">Address and port of the REMME node web sockets endpoint</param>
        /// <param name="privateKeyHex">Hex Private Key</param>
        public RemmeClient(string privateKeyHex,
                           string nodeAddress = "localhost:8080",
                           string socketAddress = "localhost:9080")
            : this(new RemmeAccount(privateKeyHex), nodeAddress, socketAddress) { }

        /// <summary>
        /// Initialize new instance of RemmeClient
        /// </summary>
        /// <param name="nodeAddress">Address and port of the REMME node Rest API</param>
        /// <param name="socketAddress">Address and port of the REMME node web sockets endpoint</param>
        /// <param name="privateKeyBytes">Private Key Bytes</param>
        public RemmeClient(byte[] privateKeyBytes,
                           string nodeAddress = "localhost:8080",
                           string socketAddress = "localhost:9080")
            : this(new RemmeAccount(privateKeyBytes), nodeAddress, socketAddress) { }

        /// <summary>
        /// Initialize new instance of RemmeClient
        /// </summary>
        /// <param name="nodeAddress">Address and port of the REMME node Rest API</param>
        /// <param name="socketAddress">Address and port of the REMME node web sockets endpoint</param>
        public RemmeClient(string nodeAddress = "localhost:8080",
                           string socketAddress = "localhost:9080")
            : this(new RemmeAccount(), nodeAddress, socketAddress) { }

        /// <summary>
        /// Initialize new instance of RemmeClient
        /// </summary>
        /// <param name="nodeAddress">Address and port of the REMME node Rest API</param>
        /// <param name="socketAddress">Address and port of the REMME node web sockets endpoint</param>
        /// <param name="remmeAccount">Remme Account object which should incapsulate keys data</param>
        public RemmeClient(RemmeAccount remmeAccount,
                           string nodeAddress,
                           string socketAddress)
        {
            _remmeRest = new RemmeRest(nodeAddress, socketAddress);
            Account = remmeAccount;
            _remmeTransactionService = new RemmeTransactionService(Account, _remmeRest);
            PublicKeyStorage = new RemmePublicKeyStorage(_remmeRest, _remmeTransactionService);
            Certificate = new RemmeCertificate(PublicKeyStorage);
            Token = new RemmeToken(_remmeRest, _remmeTransactionService);
            Batch = new RemmeBatch(_remmeRest);
        }

        public IRemmeCertificate Certificate { get; private set; }
        public IRemmeToken Token { get; private set; }
        public IRemmeAccount Account { get; private set; }
        public IRemmeBatch Batch { get; private set; }
        public IRemmePublicKeyStorage PublicKeyStorage { get; private set; }

        /// <summary>
        /// Genarates new Remme Account
        /// </summary>
        /// <returns>Remme account object wich encapulates signing logic and key pair</returns>
        public static IRemmeAccount GenerateNewAccount()
        {
            return new RemmeAccount();
        }
    }
}
