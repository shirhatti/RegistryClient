﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RegistryClient
{
    public class AcrTokenService : ITokenService
    {
        public Task<BearerToken> GetTokenAsync(AuthenticationChallenge challenge)
        {
            throw new NotImplementedException();
        }
    }
}
