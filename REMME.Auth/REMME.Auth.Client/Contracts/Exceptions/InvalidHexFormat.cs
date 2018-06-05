using System;

namespace REMME.Auth.Client.Contracts.Exceptions
{
    public class InvalidHexFormat : Exception
    {
        public InvalidHexFormat(string message) : base(message)
        {
        }
    }
}
