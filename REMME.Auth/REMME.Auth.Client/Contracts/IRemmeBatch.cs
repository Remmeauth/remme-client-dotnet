using REMME.Auth.Client.RemmeApi.Models.Batch;
using System.Threading.Tasks;

namespace REMME.Auth.Client.Contracts
{
    public interface IRemmeBatch
    {
        Task<BatchStatusResult> GetStatus(string batchId);
    }
}
