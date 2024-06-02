using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReklamationAPI.Authentication;
using ReklamationAPI.dto;
using ReklamationAPI.responses;
using ReklamationAPI.Swagger;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace ReklamationAPI.Controllers
{
    /// <summary>
    /// Controller for login API.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IUserManager<IdentityUser> userManager, IAuthService authService) : ControllerBase
    {
        private readonly IUserManager<IdentityUser> _userManager = userManager;
        private readonly IAuthService _authService = authService;

        // POST: api/login
        /// <summary>
        /// Performs a login with username and password to retrieve an authentification token.
        /// </summary>
        /// <param name="loginDto">The login information.</param>
        /// <returns>The authentification token.</returns>
        /// <response code="200">OK if the login is successful.</response>
        /// <response code="401">Unauthorized if the login fails.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(LoginResponseExample))]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                var token = _authService.GenerateJwtToken(user, roles);
                return Ok(new { token });
            }

            return Unauthorized(new
            {
                message = "Invalid user password combination.",
                user = loginDto.Username,
                password = loginDto.Password
            });
        }
    }
}
