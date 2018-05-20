using REMME.Auth.Client.Contracts;
using System;
using System.Threading.Tasks;
using REMME.Auth.Client.Contracts.Models;

namespace REMME.Auth.Client.Implementation
{
    public class RemmePersonal : IRemmePersonal
    {
        public RemmeAccountDto GenerateAccount()
        {
            throw new NotImplementedException();
        }

        public string GetAddress()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetBalance()
        {
            throw new NotImplementedException();
        }
    }
}
