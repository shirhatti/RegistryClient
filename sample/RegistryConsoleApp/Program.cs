using System;
using System.Threading.Tasks;
using RegistryClient;

namespace RegistryConsoleApp
{
    class Program
    {
        static async Task<int> Main()
        {
            var repo = "microsoft/iis";
            var dockerHubRegistry = new Registry();
            var manifestList = await dockerHubRegistry.GetManifestListAsync(repo);
            foreach(var manifestSummary in manifestList.Manifests)
            {
                var manifest = await dockerHubRegistry.GetManifestAsync(repo, manifestSummary.Digest);
                Console.WriteLine($"{repo}:{manifestSummary.Digest}");
                Console.WriteLine($"{manifestSummary.Platform.Os}: {manifestSummary.Platform.OsVersion}");
                foreach (var layer in manifest.Layers)
                {
                    Console.WriteLine($"\t{layer.MediaType}\n\t{layer.Size}\n\t{layer.Digest}\n");
                }
            }
            return 0;
        }
    }
}
