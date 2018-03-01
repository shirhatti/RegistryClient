using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RegistryClient
{
    public class Registry : IRegistry
    {
        private static readonly Uri _dockerHubUri = new Uri("https://registry.hub.docker.com/");
        private readonly Uri _registryUri;
        private readonly HttpClient _httpClient;
        public Registry() : this(_dockerHubUri, new TokenService())
        { }
        public Registry(Uri registryUri, ITokenService tokenService)
        {
            _registryUri = registryUri;
            _httpClient = new HttpClient(new RegistryHandler(new HttpClientHandler(), tokenService));
        }

        public async Task<ApiVersion> GetApiVersionAsync()
        {
            var uri = new Uri(_registryUri, "/v2");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await _httpClient.SendAsync(request);

            if(response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // This should be unreachable code
                throw new InvalidOperationException();
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return ApiVersion.v2;
            }
            
            return ApiVersion.v1;
        }

        public async Task<IEnumerable<string>> GetTagsAsync(string name)
        {
            var uri = new Uri(_registryUri, $"/v2/{name}/tags/list");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await _httpClient.SendAsync(request);
            var responseJObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var exceptions = new List<Exception>();
                foreach (JObject error in (JArray)responseJObject["errors"])
                {
                    exceptions.Add(new RegistryException((string)error["message"], (string)error["message"], (string)error["message"]));
                }
                throw new AggregateException(exceptions);
            }
            var tags = ((JArray)responseJObject["tags"]).ToObject<List<string>>();
            return tags;
        }
    }
}
