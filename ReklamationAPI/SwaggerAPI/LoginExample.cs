using ReklamationAPI.dto;
using Swashbuckle.AspNetCore.Filters;

namespace ReklamationAPI.Swagger
{
    /// <summary>
    /// Class providing an example login dto responses for swagger response documentation.
    /// </summary>
    public class LoginExample : IExamplesProvider<LoginDto>
    {

        public LoginDto GetExamples()
        {
            var loginData = new LoginDto("admin", "Admin!123");
            return loginData;
        }
    }
}
