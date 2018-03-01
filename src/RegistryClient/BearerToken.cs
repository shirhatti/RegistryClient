using System;

namespace RegistryClient
{
    public class BearerToken
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }

        public BearerToken()
        { }
        public BearerToken(string Token, DateTime Expiration)
        {
            this.Token = Token;
            this.Expiration = Expiration;
        }
    }
}