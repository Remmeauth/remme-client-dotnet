using REMME.Auth.Client.Contracts;
using System.Threading.Tasks;
using REMME.Auth.Client.RemmeApi.Models.Batch;
using REMME.Auth.Client.RemmeApi;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeBatch : IRemmeBatch
    {
        private readonly IRemmeRest _remmeRest;

        public RemmeBatch(IRemmeRest remmeRest)
        {
            _remmeRest = remmeRest;
        }

        public async Task<BatchStatusResult> GetStatus(string batchId)
        {
            return await _remmeRest.GetRequest<BatchStatusResult>(RemmeMethodsEnum.BatchStatus, batchId);
        }
    }
}
