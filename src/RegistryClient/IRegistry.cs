using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegistryClient
{
    public interface IRegistry
    {
        Task<ApiVersion> GetApiVersionAsync();
        Task<IList<string>> GetTagsAsync(string name);
        Task<string> GetDigestFromTagAsync(string name, string reference);
        Task<Manifest> GetManifestAsync(string name, string reference);
        Task<string> PutManifestAsync(string name, string tag, Manifest manifest);
        Task<ManifestList> GetManifestListAsync(string name, string reference);
    }
}