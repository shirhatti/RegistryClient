using System.Threading.Tasks;

namespace RegistryClient
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync(AuthenticationChallenge challenge);
    }
}
