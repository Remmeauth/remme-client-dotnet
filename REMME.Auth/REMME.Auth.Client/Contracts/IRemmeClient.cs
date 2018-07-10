namespace REMME.Auth.Client.Contracts
{
    interface IRemmeClient
    {
        IRemmeCertificate Certificate { get; }

        IRemmeToken Token { get; }

        IRemmeAccount Account { get; }

        IRemmeBatch Batch { get; }

        IRemmePublicKeyStorage PublicKeyStorage { get; }

        IRemmeAtomicSwap AtomicSwap { get; }
    }
}
