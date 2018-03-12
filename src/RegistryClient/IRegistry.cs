using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegistryClient
{
    public interface IRegistry
    {
        Task<ApiVersion> GetApiVersionAsync();

        Task<PagedResult<string>> GetRepositoriesAsync();
        Task<IList<string>> GetTagsAsync(string name);
        Task<Manifest> GetManifestAsync(string name, string reference);
        Task<ManifestList> GetManifestListAsync(string name, string reference);
    }
}