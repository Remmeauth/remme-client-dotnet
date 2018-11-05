using System.Threading.Tasks;

namespace REMME.Auth.Client.RemmeApi
{
    public interface IRemmeApi
    {
        string ApiAddress { get; }
        string SocketAddress { get; }

        Task<Output> SendRequest<Output>(RemmeMethodsEnum method);

        Task<Output> SendRequest<Input, Output>(RemmeMethodsEnum method, Input requestPayload);
    }
}
