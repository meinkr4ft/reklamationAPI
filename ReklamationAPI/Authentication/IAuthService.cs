using Microsoft.AspNetCore.Identity;

namespace ReklamationAPI.Authentication
{
    /// <summary>
    /// Interface for abstraction and testability.
    /// </summary>
    public interface IAuthService
    {
        public string GenerateJwtToken(IdentityUser user, IList<string> roles);
    }
}
