using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RegistryClient.DockerHub;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RegistryClient
{
    public class Registry : IRegistry
    {
        private static readonly Uri _dockerHubUri = new Uri("https://registry.hub.docker.com/");
        private readonly Uri _registryUri;
        private readonly HttpClient _httpClient;
        public Registry() : this(_dockerHubUri, new DockerHubTokenService())
        { }
        public Registry(Uri registryUri, ITokenService tokenService)
        {
            _registryUri = registryUri;
            _httpClient = new HttpClient(new RegistryHandler(new HttpClientHandler(), tokenService));
        }
        /// <summary>
        /// Retrieve the API version of the Docker Registry HTTP API
        /// </summary>
        /// <returns>Returns the <see cref="ApiVersion" /> of the Docker Registry</returns>
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
        /// <summary>
        /// Fetch the tags under the repository identified by <paramref name="name"/>
        /// </summary>
        /// <param name="name">Name of the target repository</param>
        /// <returns>Returns an <see cref="IList{String}" /> of tags for the named repository</returns>
        public async Task<IList<string>> GetTagsAsync(string name)
        {
            var uri = new Uri(_registryUri, $"/v2/{name}/tags/list");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                var responseJObject = JObject.Parse(responseContent);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw CreateException(responseJObject);
                }
                var tags = ((JArray)responseJObject["tags"]).ToObject<List<string>>();
                return tags;
            }
            catch (JsonReaderException e)
            {
                throw new RegistryException(e.Message);
            }
        }
        /// <summary>
        /// Fetch the manifest identified by <paramref name="name"/> and <paramref name="reference"/>
        /// </summary>
        /// <param name="name">Name of the target repository</param>
        /// <param name="reference">Tag or digeset of the target manifest</param>
        /// <returns>Returns a <see cref="Manifest" /> for the specified reference</returns>
        public async Task<Manifest> GetManifestAsync(string name, string reference = "latest")
        {
            var uri = new Uri(_registryUri, $"/v2/{name}/manifests/{reference}");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.docker.distribution.manifest.v2+json"));
            var response = await _httpClient.SendAsync(request);
            var responseJObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw CreateException(responseJObject);
            }
            return responseJObject.ToObject<Manifest>();
        }

        public async Task<ManifestList> GetManifestListAsync(string name)
        {
            return await GetManifestListAsync(name, "latest");
        }
        public async Task<ManifestList> GetManifestListAsync(string name, string reference)
        {
            var uri = new Uri(_registryUri, $"/v2/{name}/manifests/{reference}");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.docker.distribution.manifest.list.v2+json"));
            var response = await _httpClient.SendAsync(request);
            var responseJObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw CreateException(responseJObject);
            }
            return responseJObject.ToObject<ManifestList>();
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
