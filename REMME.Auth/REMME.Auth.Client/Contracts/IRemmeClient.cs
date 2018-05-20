namespace REMME.Auth.Client.Contracts
{
    interface IRemmeClient
    {
        IRemmeCertificate Certificate { get; }

        IRemmeToken Token { get; }

        IRemmePersonal Personal { get; }
    }
}
