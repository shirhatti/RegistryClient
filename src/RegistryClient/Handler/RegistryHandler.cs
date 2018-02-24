using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace RegistryClient
{
    public class RegistryHandler : DelegatingHandler
    {
        public Dictionary<string, BearerToken> TokenCache { get; private set; }
        public RegistryHandler(HttpMessageHandler messageHandler)
            : base (messageHandler)
        { }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage requestMessage,
            CancellationToken cancellationToken
        )
        {
            // Add bearer token

            var response = await base.SendAsync(requestMessage, cancellationToken);
            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            // If we get a 401, do the OAuth workflow
            var wwwAuthenticate = response.Headers.WwwAuthenticate.FirstOrDefault();
            if (!wwwAuthenticate.Scheme.Equals("Bearer"))
            {
                throw new NotSupportedException();
            }

            // TODO eliminate ugly regex hack
            Regex regex = new Regex("^(?:(?:[, ]+)?(?\'q\'\")?(?\'key\'[^=\"]*?)(?:\\k\'q\'(?\'-q\'))?=(?\'q\'\")?(?\'value\'(?:[^\"]|(?<=\\\\)\")*)(?:\\k\'q\'(?\'-q\'))?)*(?(q)(?!))$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(10));
            var match = regex.Match(wwwAuthenticate.Parameter);
            //var keys = match.Groups["key"].Captures;
            var values = match.Groups["value"].Captures;
            var realm = values[0].Value;
            var service = values[1].Value;

            await GenerateNewTokenAsync(service, realm);

            // Retry with bearer token
            var bearerToken = new BearerToken();
            TokenCache.TryGetValue(service, out bearerToken);
            requestMessage.Headers.Add("Authorization", $"Bearer {bearerToken.Token}");
            var response2 = await base.SendAsync(requestMessage, cancellationToken);

            return response2;
            
        }

        private async Task GenerateNewTokenAsync(string service, string realm)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["service"] = service;
            var uriBuilder = new UriBuilder(realm);
            uriBuilder.Query = query.ToString();
            var uri = uriBuilder.ToString();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            var cancellationToken = new CancellationToken();
            var responseMessage = await base.SendAsync(requestMessage, cancellationToken);
            var responseJObject = JObject.Parse(await responseMessage.Content.ReadAsStringAsync());
            var token = (string)responseJObject["token"];
            var issuedAt = (string)responseJObject["issued_at"];
            var time = new DateTime();
            DateTime.TryParse(issuedAt, out time);
            var expiresIn = (int)responseJObject["expires_in"];
            time.AddSeconds(expiresIn);
            var bearerToken = new BearerToken(token, time);

            TokenCache.Add(service, bearerToken);
        }
    }
}