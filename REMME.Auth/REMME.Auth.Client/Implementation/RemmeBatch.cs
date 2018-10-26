using REMME.Auth.Client.Contracts;
using System.Threading.Tasks;
using REMME.Auth.Client.RemmeApi.Models.Batch;
using REMME.Auth.Client.RemmeApi;
using System;

namespace REMME.Auth.Client.Implementation
{
    public class RemmeBatch : IRemmeBatch
    {
        private readonly IRemmeApi _remmeRest;

        public RemmeBatch(IRemmeApi remmeRest)
        {
            _remmeRest = remmeRest;
        }

        public async Task<BatchStatusResult> GetStatus(string batchId)
        {
            var statusString = await _remmeRest
                            .SendRequest<GetBatchStatusRequest, string>
                            (RemmeMethodsEnum.GetBatchStatus,
                             new GetBatchStatusRequest { BatchId = batchId });

            return new BatchStatusResult { BatchId = batchId, StatusString = statusString };
        }
    }
}
