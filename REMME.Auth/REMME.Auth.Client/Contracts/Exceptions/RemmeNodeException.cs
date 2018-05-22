using System;

namespace REMME.Auth.Client.Contracts.Exceptions
{
    public class RemmeNodeException : Exception
    {
        public RemmeNodeException(string message) : base(message)
        {
        }
    }
}
