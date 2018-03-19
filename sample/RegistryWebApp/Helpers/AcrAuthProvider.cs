using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegistryWebApp.Helpers
{
    public class AcrAuthProvider
    {
        private readonly IMemoryCache _memoryCache;
        private readonly string _appId;
        //private readonly ClientCredential _credential;
        public AcrAuthProvider(IMemoryCache memoryCache, IConfiguration configuration)
        {
            var azureOptions = new AzureAdOptions();
            configuration.Bind("AzureAd", azureOptions);

            _appId = azureOptions.ClientId;
            //_credential = new ClientCredential(azureOptions.ClientSecret);

            _memoryCache = memoryCache;
        }
    }
}
