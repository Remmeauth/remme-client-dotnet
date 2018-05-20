using REMME.Auth.Client.Contracts.Models;
using System.Threading.Tasks;

namespace REMME.Auth.Client.Contracts
{
    /// <summary>
    /// Helper interface for interacting with account with wich RemmeClient is initialized
    /// </summary>
    public interface IRemmePersonal
    {
        Task<int> GetBalance();

        string GetAddress();

        RemmeAccountDto GenerateAccount();
    }
}
