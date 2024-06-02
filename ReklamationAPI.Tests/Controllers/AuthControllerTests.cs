using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReklamationAPI.Authentication;
using ReklamationAPI.dto;
using ReklamationAPI.Tests;

namespace ReklamationAPI.Controllers.Tests
{
    [TestClass()]
    public class AuthControllerTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private AuthController _authController;
        private Mock<IUserManager<IdentityUser>> _mockUserManager;
        private Mock<IAuthService> _mockAuthService;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [TestInitialize]
        public void Setup()
        {
            _mockUserManager = new Mock<IUserManager<IdentityUser>>();


            _mockAuthService = new Mock<IAuthService>();
            _authController = new AuthController(_mockUserManager.Object, _mockAuthService.Object);
        }

        [TestMethod]
        public async Task Login_ValidCredentials_ReturnsOkObjectResultWithToken()
        {
            // Arrange
            var loginDto = new LoginDto("validUsername", "validPassword");
            var user = new IdentityUser { UserName = loginDto.Username };
            var roles = new List<string> { "admin", "user" };
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzE3MTk5MzIyLCJleHAiOjE3MTc4MDQxMjIsImlhdCI6MTcxNzE5OTMyMiwiaXNzIjoiUmVrbGFtYXRpb25BUElJc3N1ZXIiLCJhdWQiOiJSZWtsYW1hdGlvbkFQSUF1ZGllbmNlIn0.HGzO7GjP5XP234B49EerLrqAHX6fOAGbHaEsJOh1yg0";

            _mockUserManager.Setup(m => m.FindByNameAsync(loginDto.Username))
                .ReturnsAsync(user);
            _mockUserManager.Setup(m => m.CheckPasswordAsync(user, loginDto.Password))
                .ReturnsAsync(true);
            _mockUserManager.Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync(roles);
            _mockAuthService.Setup(m => m.GenerateJwtToken(user, roles))
                .Returns(token);

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var data = okResult.Value;
            Assert.IsNotNull(data);
            var tokenValue = data.GetValue<string>("token");
            Assert.IsNotNull(tokenValue);
            Assert.AreEqual(tokenValue, token);
        }

        [TestMethod]
        public async Task Login_InvalidCredentials_ReturnsUnauthorizedObjectResult()
        {
            // Arrange
            var loginDto = new LoginDto("invalidUsername", "invalidPassword");

            _mockUserManager.Setup(m => m.FindByNameAsync(loginDto.Username))
                .ReturnsAsync((IdentityUser?)null);

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType<UnauthorizedObjectResult>(result);
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.IsNotNull(unauthorizedResult.Value);
            var message = unauthorizedResult.Value.GetValue<string>("message");
            Assert.IsNotNull(message);
            Assert.AreEqual(message, "Invalid user password combination.");
        }
    }
}