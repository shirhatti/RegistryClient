using System.Threading.Tasks;

namespace RegistryClient
{
    public interface ITokenService
    {
        Task<BearerToken> GetTokenAsync(AuthenticationChallenge challenge);
    }
}