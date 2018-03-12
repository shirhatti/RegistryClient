using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace RegistryClient.AzureContainerRegistry
{
    public class AzureContainerRegistryTokenService : ITokenService
    {
        private Uri _registryUri;
        private static HttpClient _client = new HttpClient();

        public AzureContainerRegistryTokenService(Uri RegistryUri)
        {
            _registryUri = RegistryUri;
        }

        public async Task<BearerToken> GetTokenAsync(AuthenticationChallenge challenge)
        {
            if (!challenge.Realm.Host.Equals(_registryUri.Host))
            {
                throw new RegistryException();
            }

            var uri = new Uri(_registryUri, "/oauth2/exchange");
            var tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
            // accessToken = (Get - Content - Raw - Path $HOME /.azure / accessTokens.json | ConvertFrom - Json)[1].accessToken | Set-Clipboard
            // refreshToken = (Get-Content -Raw -Path $HOME/.azure/accessTokens.json | ConvertFrom-Json)[1].refreshToken | Set-Clipboard
            // tenantId = (Get-Content -Raw -Path $HOME/.azure/accessTokens.json | ConvertFrom-Json)[1]._authority.Split("/")[-1] | Set-Clipboard
                        var content = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "access_token_refresh_token"),
                new KeyValuePair<string, string>("service", _registryUri.Host),
                new KeyValuePair<string, string>("tenant", tenantId),
                new KeyValuePair<string, string>("aad_access_token", "eyJ...H-g"),
                new KeyValuePair<string, string>("aad_refresh_token", "AQA..IAA")
            };
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new FormUrlEncodedContent(content)
            };
            var cancellationToken = new CancellationToken();
            var responseMessage = await _client.SendAsync(requestMessage, cancellationToken);
            var responseMessageString = await responseMessage.Content.ReadAsStringAsync();

            throw new NotImplementedException();
        }
    }
}
