using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RegistryClient.AzureContainerRegistry
{
    public class AzureContainerRegistryTokenService : ITokenService
    {
        private readonly Uri _registryUri;
        private static HttpClient _client = new HttpClient();
        private readonly string _accessToken;
        private readonly string _refreshToken;

        public AzureContainerRegistryTokenService(Uri RegistryUri, string AccessToken, string RefreshToken)
        {
            _registryUri = RegistryUri;
            _accessToken = AccessToken;
            _refreshToken = RefreshToken;
        }

        public async Task<string> GetAcrRefreshToken(AuthenticationChallenge challenge)
        {
            var uri = new Uri(_registryUri, @"/oauth2/exchange");
            // TODO Make the tenant ID configurable
            var tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";

            var content = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "access_token_refresh_token"),
                new KeyValuePair<string, string>("service", _registryUri.Host),
                new KeyValuePair<string, string>("tenant", tenantId),
                // accessToken = (Get - Content - Raw - Path $HOME /.azure / accessTokens.json | ConvertFrom - Json)[1].accessToken | Set-Clipboard
                new KeyValuePair<string, string>("access_token", _accessToken),
                // refreshToken = (Get-Content -Raw -Path $HOME/.azure/accessTokens.json | ConvertFrom-Json)[1].refreshToken | Set-Clipboard
                new KeyValuePair<string, string>("refresh_token", _refreshToken)
            };
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new FormUrlEncodedContent(content)
            };

            var response = await _client.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                var responseJObject = JObject.Parse(responseContent);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw CreateException(responseJObject);
                }
                var refreshToken = responseJObject["refresh_token"].ToString();
                return refreshToken;
            }
            catch (JsonReaderException e)
            {
                throw new RegistryException(e.Message);
            }
        }

        public async Task<string> GetTokenAsync(AuthenticationChallenge challenge)
        {
            var acrRefreshToken = await GetAcrRefreshToken(challenge);
            string bearerToken;
            var key = challenge.ToString();
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
            requestMessage.AddNetworkCredential("00000000-0000-0000-0000-000000000000", acrRefreshToken);

            var responseMessage = _client.SendAsync(requestMessage);
            var responseJObject = JObject.Parse(await (await responseMessage).Content.ReadAsStringAsync());
            bearerToken = (string)responseJObject["access_token"];

            return bearerToken;
        }

        private Exception CreateException(JObject httpResponse)
        {
            var exceptions = new List<Exception>();
            foreach (JObject error in (JArray)httpResponse["errors"])
            {
                exceptions.Add(new RegistryException((string)error["message"], (string)error["message"], (string)error["message"]));
            }
            if (exceptions.Count == 1)
            {
                return exceptions[0];
            }
            return new AggregateException(exceptions);
        }
    }
}
