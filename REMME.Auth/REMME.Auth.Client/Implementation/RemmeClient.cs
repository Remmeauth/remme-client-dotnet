using REMME.Auth.Client.Contracts;
using REMME.Auth.Client.RemmeApi;
using System;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeClient : IRemmeClient
    {
        private readonly RemmeRest _remmeRest;

        /// <remarks>
        /// pathToKeyStore is not supported yet. 
        /// It will be supported after raw transactions 
        /// will be implemented in future releases
        /// </remarks>
        /// <summary>
        /// Initialize new instance of RemmeClient
        /// </summary>
        /// <param name="nodeAddress">Address and port of the REMME node Rest API</param>
        /// <param name="socketAddress">Address and port of the REMME node web sockets endpoint</param>
        /// <param name="pathToKeyStore">Path to file with keystore. NOT IMPLEMENTED YET</param>
        public RemmeClient(string nodeAddress = "localhost:8080",
                           string socketAddress = "localhost:9080",
                           string pathToKeyStore = "")
        {
            _remmeRest = new RemmeRest(nodeAddress, socketAddress);

            Certificate = new RemmeCertificate(_remmeRest);
            Token = new RemmeToken(_remmeRest);
            Batch = new RemmeBatch(_remmeRest);
            Personal = new RemmePersonal();
        }

        public IRemmeCertificate Certificate { get; private set; }
        public IRemmeToken Token { get; private set; }
        public IRemmePersonal Personal { get; private set; }
        public IRemmeBatch Batch { get; private set; }
    }
}
