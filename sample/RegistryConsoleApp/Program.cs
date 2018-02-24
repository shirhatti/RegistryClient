using System;
using System.Threading.Tasks;
using RegistryClient;

namespace RegistryConsoleApp
{
    class Program
    {
        static async Task<int> Main()
        {
            var dockerHubRegistry = new Registry();
            await dockerHubRegistry.ConnectAsync();
            return 0;
        }
    }
}
