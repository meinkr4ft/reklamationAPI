using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ReklamationAPI.Authentication
{
    public static class AuthConfig
    {

        /// <summary>
        /// Audience used for authentification token.
        /// </summary>
        public const string ApiJwtAudience = "ReklamationAPIAudience";

        /// <summary>
        /// Issuer used for authentification token.
        /// </summary>
        public const string ApiJwtIssuer = "ReklamationAPIIssuer";

        /// <summary>
        /// Lifetime used for authentification token (1 day).
        /// </summary>
        public const int ApiJwtExpirationSec = 60 * 60 * 24;

        /// <summary>
        /// Symmetric security key string used for generating the key.
        /// </summary>
        private const string ApiSecurityTokenPass = "o12TWIcH7DjOgoh5eSXUvftWw7clHdKxM+LkRFHwurA=";

        /// <summary>
        /// Symmetric security key property.
        /// </summary>
        private static SymmetricSecurityKey? signingKey;

        /// <summary>
        /// Symmetric security key used for authentification token.
        /// </summary>
        public static SymmetricSecurityKey ApiJwtSigningKey
        {
            get
            {
                if (signingKey == null)
                {
                    byte[] key = Encoding.UTF8.GetBytes(ApiSecurityTokenPass, 0, 32);
                    signingKey = new SymmetricSecurityKey(key);
                }
                return signingKey;
            }
        }
    }
}
