using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace RegistryClient
{
    public static class HttpRequestMessageExtensions
    {
        public static HttpRequestMessage AddNetworkCredential(this HttpRequestMessage httpRequestMessage, NetworkCredential networkCredential)
        {
            var credential = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{networkCredential.UserName}:{networkCredential.Password}"));
            var authHeaderValue = new AuthenticationHeaderValue("Basic", credential);
            httpRequestMessage.Headers.Authorization = authHeaderValue;

            return httpRequestMessage;
        }
    }
}

