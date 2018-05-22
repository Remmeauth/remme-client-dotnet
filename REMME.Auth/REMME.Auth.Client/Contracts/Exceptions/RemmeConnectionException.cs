using System;
namespace REMME.Auth.Client.Contracts.Exceptions
{
    public class RemmeConnectionException : Exception
    {
        public RemmeConnectionException(string message) : base(message)
        {
        }
    }
}
