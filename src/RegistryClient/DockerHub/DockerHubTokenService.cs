using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace RegistryClient
{
    public class DockerHubTokenService : ITokenService
    {
        private static HttpClient _client = new HttpClient();
        private static NetworkCredential _credential;
        private readonly IMemoryCache _cache;
        public DockerHubTokenService()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public DockerHubTokenService(NetworkCredential credential) : this()
        {
            _credential = credential;
        }

        public async Task<string> GetTokenAsync(AuthenticationChallenge challenge)
        {
            string bearerToken;
            var key = challenge.ToString();
            if (!_cache.TryGetValue(key, out bearerToken))
            {
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["service"] = challenge.Service;
                if (challenge.Scope != null)
                {
                    query["scope"] = challenge.Scope;
                }
                var uriBuilder = new UriBuilder(challenge.Realm);
                uriBuilder.Query = query.ToString();
                var uri = uriBuilder.ToString();
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
                var cancellationToken = new CancellationToken();
                if (_credential != null)
                {
                    requestMessage.AddNetworkCredential(_credential);
                }

                var responseMessage = _client.SendAsync(requestMessage, cancellationToken);
                var responseJObject = JObject.Parse(await (await responseMessage).Content.ReadAsStringAsync());
                bearerToken = (string)responseJObject["token"];
                var expiresIn = (int)responseJObject["expires_in"];
                _cache.Set(key,
                    bearerToken,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddSeconds(expiresIn)));

            }
            return bearerToken;
        }
    }
}
