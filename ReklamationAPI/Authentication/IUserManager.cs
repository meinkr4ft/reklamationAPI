namespace ReklamationAPI.Authentication
{
    /// <summary>
    /// Interface for abstraction and testability.
    /// </summary>
    public interface IUserManager<TUser> : IDisposable where TUser : class
    {
        public Task<TUser?> FindByNameAsync(string userName);
        public Task<bool> CheckPasswordAsync(TUser user, string password);
        public Task<IList<string>> GetRolesAsync(TUser user);
        new void Dispose();
    }
}
