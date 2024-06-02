using Microsoft.AspNetCore.Identity;

namespace ReklamationAPI.Authentication
{
    /// <summary>
    /// Wrapper Implementation for abstraction and testability.
    /// </summary>
    public class UserManagerWrapper(UserManager<IdentityUser> userManager) : IUserManager<IdentityUser>
    {
        readonly UserManager<IdentityUser> UserManager = userManager;

        public async Task<bool> CheckPasswordAsync(IdentityUser user, string password)
        {
            return await this.UserManager.CheckPasswordAsync(user, password);
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            this.UserManager.Dispose();
        }

        public async Task<IdentityUser?> FindByNameAsync(string userName)
        {
            return await this.UserManager.FindByNameAsync(userName);
        }

        public async Task<IList<string>> GetRolesAsync(IdentityUser user)
        {
            return await this.UserManager.GetRolesAsync(user);
        }
    }
}
