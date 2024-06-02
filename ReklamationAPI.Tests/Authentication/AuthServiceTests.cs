using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Text.Json;

namespace ReklamationAPI.Authentication.Tests
{
    [TestClass()]
    public class AuthServiceTests
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        [TestMethod()]
        public void GenerateJwtToken_ReturnsCorrectAdminToken()
        {
            // Arange
            var authService = new AuthService();
            var roles = new[] { "admin", "user" };
            var identityUserAdmin = new IdentityUser("admin");

            // Act
            var token = authService.GenerateJwtToken(identityUserAdmin, roles);


            // Assert
            var parts = token.Split(".");
            Assert.AreEqual(parts.Length, 3);

            var jsonString = GetPayloadJson(token);
            var jsonObjectAdmin = JsonSerializer.Deserialize<TokenJsonAdmin>(jsonString, options);
            Assert.IsNotNull(jsonObjectAdmin);
            Assert.AreEqual("admin", jsonObjectAdmin.Unique_name);
            Assert.IsNotNull(jsonObjectAdmin.Role);
            Assert.IsTrue(roles.SequenceEqual(jsonObjectAdmin.Role));
            Assert.IsNotNull(jsonObjectAdmin.Nbf);
            Assert.IsNotNull(jsonObjectAdmin.Exp);
            Assert.IsNotNull(jsonObjectAdmin.Iat);
            Assert.AreEqual("ReklamationAPIIssuer", jsonObjectAdmin.Iss);
            Assert.AreEqual("ReklamationAPIAudience", jsonObjectAdmin.Aud);

        }

        [TestMethod()]
        public void GenerateJwtToken_ReturnsCorrectUserToken()
        {
            // Arange
            var authService = new AuthService();
            var roles = new[] { "user" };
            var identityUserAdmin = new IdentityUser("user");

            // Act
            var token = authService.GenerateJwtToken(identityUserAdmin, roles);


            // Assert
            var parts = token.Split(".");
            Assert.AreEqual(parts.Length, 3);
            var jsonString = GetPayloadJson(token);
            var jsonObjectUser = JsonSerializer.Deserialize<TokenJsonUser>(jsonString, options);
            Assert.IsNotNull(jsonObjectUser);
            Assert.AreEqual("user", jsonObjectUser.Unique_name);
            Assert.AreEqual("user", jsonObjectUser.Role);
            Assert.IsNotNull(jsonObjectUser.Nbf);
            Assert.IsNotNull(jsonObjectUser.Exp);
            Assert.IsNotNull(jsonObjectUser.Iat);
            Assert.AreEqual("ReklamationAPIIssuer", jsonObjectUser.Iss);
            Assert.AreEqual("ReklamationAPIAudience", jsonObjectUser.Aud);
        }

        /// <summary>
        /// Helper method extracting and padding the body of a token and padding it if needed.
        /// </summary>
        /// <param name="token">Token to be extracted from.</param>
        /// <returns>Token payload</returns>
        private static string GetPayloadJson(string token)
        {
            string base64Url = token.Split(".")[1];
            string base64 = base64Url.Replace('-', '+').Replace('_', '/');

            // Add padding characters if necessary
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            var jsonBytes = Convert.FromBase64String(base64);
            var jsonString = Encoding.UTF8.GetString(jsonBytes);
            return jsonString;
        }

        /// <summary>
        /// Base class for deserializing json.
        /// </summary>
        public abstract class TokenJsonBase
        {
            public string? Unique_name { get; set; }
            public uint Nbf { get; set; }
            public uint Exp { get; set; }
            public uint Iat { get; set; }
            public string? Iss { get; set; }
            public string? Aud { get; set; }
        }

        /// <summary>
        /// Admin token class for deserializing json.
        /// </summary>
        public class TokenJsonAdmin : TokenJsonBase
        {
            public string[]? Role { get; set; }
        }

        /// <summary>
        /// User token class for deserializing json.
        /// </summary>
        public class TokenJsonUser : TokenJsonBase
        {

            public string? Role { get; set; }
        }
    }
}