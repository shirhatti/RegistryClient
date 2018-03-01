using System;
using System.Runtime.Serialization;

namespace RegistryClient
{
    [Serializable]
    public class RegistryException : Exception
    {
        public string Code { get; set; }
        public string Detail { get; set; }

        public RegistryException()
        {
        }

        public RegistryException(string message) : base(message)
        {
        }

        public RegistryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RegistryException(string message, string code, string detail) : this(message)
        {
            this.Code = code;
            this.Detail = detail;
        }
    }
}