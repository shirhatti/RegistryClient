using System;
using System.Collections.Generic;
using System.Text;

namespace RegistryClient
{
    public class AuthenticationChallenge
    {
        public Uri Realm { get; set; }
        public string Service { get; set; }
        public string Scope { get; set; }
    }
}
