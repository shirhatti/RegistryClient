using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RegistryClient
{
    public class Registry : IRegistry
    {
        private static readonly Uri _dockerHubUri = new Uri("https://registry.hub.docker.com/");
        private readonly Uri _registryUri;
        private readonly HttpClient _httpClient = new HttpClient(new RegistryHandler(new HttpClientHandler()));
        public Registry()
        {
            _registryUri = _dockerHubUri; 
        }
        public Registry(Uri registryUri)
        {
            _registryUri = registryUri; 
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
        public async Task ConnectAsync(){
            var apiVersion = await GetApiVersionAsync();
            
            if (apiVersion == ApiVersion.v1)
            {
                throw new NotSupportedException();
            }
        }
    }
}
