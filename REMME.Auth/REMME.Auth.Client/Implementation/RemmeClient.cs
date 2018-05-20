using REMME.Auth.Client.Contracts;
using REMME.Auth.Client.RemmeApi;
using System;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeClient : IRemmeClient
    {
        private readonly RemmeRest _remmeRest;

        public RemmeClient(string pathToKeyStore, string nodeAddress = "localhost:8080")
        {
            _remmeRest = new RemmeRest(nodeAddress);

            this.Certificate = new RemmeCertificate(_remmeRest);
            this.Token = new RemmeToken(_remmeRest);
            this.Personal = new RemmePersonal();
        }

        public IRemmeCertificate Certificate { get; private set; }
        public IRemmeToken Token { get; private set; }
        public IRemmePersonal Personal { get; private set; }
    }
}
