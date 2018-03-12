using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RegistryClient
{
    public class RegistryHandler : DelegatingHandler
    {
        private ITokenService _tokenService;
        public RegistryHandler(HttpMessageHandler messageHandler, ITokenService tokenService)
            : base (messageHandler)
        {
            _tokenService = tokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage requestMessage,
            CancellationToken cancellationToken
        )
        {
            // Add bearer token

            var response = await base.SendAsync(requestMessage, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // If we get a 401, do the OAuth workflow
                var wwwAuthenticate = response.Headers.WwwAuthenticate.FirstOrDefault();
                if (!wwwAuthenticate.Scheme.Equals("Bearer"))
                {
                    throw new NotSupportedException();
                }

                var authenticationChallenge = AuthenticationChallenge.ParseBearerResponseChallenge(wwwAuthenticate.Parameter);

                // Retry with bearer token
                var bearerToken = await _tokenService.GetTokenAsync(authenticationChallenge);
                requestMessage.Headers.Add("Authorization", $"Bearer {bearerToken}");
                response = await base.SendAsync(requestMessage, cancellationToken);
            }

            return response;       
        }
    }
}