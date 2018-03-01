using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegistryClient
{
    public interface IRegistry
    {
        Task<ApiVersion> GetApiVersionAsync();
        Task<IEnumerable<string>> GetTagsAsync(string name);
    }
}