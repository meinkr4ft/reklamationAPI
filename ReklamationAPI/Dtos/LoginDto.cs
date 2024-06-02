namespace ReklamationAPI.dto
{
    /// <summary>
    /// Data Transfer Object for receiving Login data.
    /// </summary>
    public class LoginDto(string username, string password)
    {
        public string Username { get; set; } = username;
        public string Password { get; set; } = password;
    }
}
