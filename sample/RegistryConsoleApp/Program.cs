using System;
using System.Threading.Tasks;
using RegistryClient;
using RegistryClient.DockerHub;

namespace RegistryConsoleApp
{
    class Program
    {
        static async Task<int> Main()
        {
            var dockerHubTokenService = new DockerHubTokenService();
            var dockerHubUri = new Uri("https://registry.hub.docker.com/");
            var registry = new Registry(dockerHubUri, dockerHubTokenService);

            var repo = "microsoft/iis";
            var tags = await registry.GetTagsAsync(repo);

            foreach(var tag in tags)
            {
                Console.WriteLine(tag);
            }

            return 0;
        }
    }
}
