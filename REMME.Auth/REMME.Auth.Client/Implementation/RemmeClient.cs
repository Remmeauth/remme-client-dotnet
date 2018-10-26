using REMME.Auth.Client.Contracts;
using REMME.Auth.Client.RemmeApi;
using REMME.Auth.Client.RemmeApi.Models;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeClient : IRemmeClient
    {
        private readonly RemmeApi.IRemmeApi _remmeRest;
        private readonly IRemmeTransactionService _remmeTransactionService;

        /// <summary>
        /// Initialize new instance of RemmeClient
        /// </summary>
        /// <param name="nodeAddress">Address and port of the REMME node Rest API</param>
        /// <param name="socketAddress">Address and port of the REMME node web sockets endpoint</param>
        /// <param name="privateKeyHex">Hex Private Key</param>
        public RemmeClient(string privateKeyHex,
                           RemmeNetworkConfig remmeNetworkConfig = null)
            : this(new RemmeAccount(privateKeyHex), remmeNetworkConfig) { }

        /// <summary>
        /// Initialize new instance of RemmeClient
        /// </summary>
        /// <param name="nodeAddress">Address and port of the REMME node Rest API</param>
        /// <param name="socketAddress">Address and port of the REMME node web sockets endpoint</param>
        /// <param name="privateKeyBytes">Private Key Bytes</param>
        public RemmeClient(byte[] privateKeyBytes,
                           RemmeNetworkConfig remmeNetworkConfig = null)
            : this(new RemmeAccount(privateKeyBytes), remmeNetworkConfig) { }

        /// <summary>
        /// Initialize new instance of RemmeClient
        /// </summary>
        /// <param name="nodeAddress">Address and port of the REMME node Rest API</param>
        /// <param name="socketAddress">Address and port of the REMME node web sockets endpoint</param>
        public RemmeClient(RemmeNetworkConfig remmeNetworkConfig = null)
            : this(new RemmeAccount(), remmeNetworkConfig) { }

        /// <summary>
        /// Initialize new instance of RemmeClient
        /// </summary>
        /// <param name="nodeAddress">Address and port of the REMME node Rest API</param>
        /// <param name="socketAddress">Address and port of the REMME node web sockets endpoint</param>
        /// <param name="remmeAccount">Remme Account object which should incapsulate keys data</param>
        public RemmeClient(RemmeAccount remmeAccount, RemmeNetworkConfig remmeNetworkConfig = null)
        {
            _remmeRest = new RemmeApi.RemmeApi(remmeNetworkConfig);
            Account = remmeAccount;
            _remmeTransactionService = new RemmeTransactionService(Account, _remmeRest);
            PublicKeyStorage = new RemmePublicKeyStorage(_remmeRest, _remmeTransactionService);
            Certificate = new RemmeCertificate(PublicKeyStorage);
            Token = new RemmeToken(_remmeRest, _remmeTransactionService);
            Batch = new RemmeBatch(_remmeRest);
            AtomicSwap = new RemmeAtomicSwap(_remmeRest, _remmeTransactionService);
        }

        public IRemmeCertificate Certificate { get; private set; }
        public IRemmeToken Token { get; private set; }
        public IRemmeAccount Account { get; private set; }
        public IRemmeBatch Batch { get; private set; }
        public IRemmePublicKeyStorage PublicKeyStorage { get; private set; }
        public IRemmeAtomicSwap AtomicSwap { get; private set; }

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
