using System.Threading.Tasks;

namespace REMME.Auth.Client.RemmeApi
{
    public interface IRemmeRest
    {
        string ApiAddress { get; }
        string SocketAddress { get; }

        Task<Output> GetRequest<Output>(RemmeMethodsEnum method, string requestPayload = null);

        Task<Output> PutRequest<Input, Output>(RemmeMethodsEnum method, Input requestPayload);

        Task<Output> PostRequest<Input, Output>(RemmeMethodsEnum method, Input requestPayload);

        Task<Output> DeleteRequest<Input, Output>(RemmeMethodsEnum method, Input requestPayload);
    }
}
