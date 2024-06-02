namespace ReklamationAPI.responses
{
    /// <summary>
    /// Response class containing an authentication token.´.
    /// </summary>
    public class LoginResponse(string token)
    {
        public string Token { get; set; } = token;
    }
}
