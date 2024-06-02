using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace ReklamationAPI.Authentication
{
    public class AuthService : IAuthService
    {
        /// <summary>
        /// Generates a JWT token for the given user and roles.
        /// </summary>
        /// <param name="user">The IdentityUser for which the token is generated.</param>
        /// <param name="roles">The list of roles associated with the user.</param>
        /// <returns>The generated JWT token as a string.</returns>
        public string GenerateJwtToken(IdentityUser user, IList<string> roles)
        {
            ArgumentNullException.ThrowIfNull(user.UserName, nameof(user.UserName));
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim> {
                new(ClaimTypes.Name, user.UserName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(AuthConfig.ApiJwtSigningKey, SecurityAlgorithms.HmacSha256Signature),
                Issuer = "ReklamationAPIIssuer",
                Audience = "ReklamationAPIAudience"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
