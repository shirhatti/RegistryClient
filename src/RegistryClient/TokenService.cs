using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace RegistryClient
{
    class TokenService : ITokenService
    {
        private static HttpClient _client = new HttpClient();
        private readonly ConcurrentDictionary<AuthenticationChallenge, BearerToken> _tokenCache = new ConcurrentDictionary<AuthenticationChallenge, BearerToken>();
        public TokenService()
        { }

        public async Task<BearerToken> GetTokenAsync(AuthenticationChallenge challenge)
        {
            var bearerToken = new BearerToken();
            _tokenCache.TryGetValue(challenge, out bearerToken);
            
            if (bearerToken?.Expiration < DateTime.Now)
            {
                return bearerToken;
            }

            return await RefreshBearerTokenAsync(challenge);
        }

        private async Task<BearerToken> RefreshBearerTokenAsync(AuthenticationChallenge challenge)
        {
            _tokenCache.TryRemove(challenge, out _);

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

            var responseMessage = _client.SendAsync(requestMessage, cancellationToken);
            var responseJObject = JObject.Parse(await (await responseMessage).Content.ReadAsStringAsync());
            var token = (string)responseJObject["token"];
            var issuedAt = (string)responseJObject["issued_at"];
            var time = new DateTime();
            DateTime.TryParse(issuedAt, out time);
            var expiresIn = (int)responseJObject["expires_in"];
            time = time.AddSeconds(expiresIn);
            var bearerToken = new BearerToken(token, time);

            _tokenCache.TryAdd(challenge, bearerToken);

            return bearerToken;
        }
    }
}
