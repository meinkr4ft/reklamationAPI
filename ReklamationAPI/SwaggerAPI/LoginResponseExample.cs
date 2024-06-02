using ReklamationAPI.responses;
using Swashbuckle.AspNetCore.Filters;

namespace ReklamationAPI.Swagger
{
    /// <summary>
    /// Class providing an example login response responses for swagger response documentation.
    /// </summary>
    public class LoginResponseExample : IExamplesProvider<LoginResponse>
    {
        public LoginResponse GetExamples()
        {
            var response = new LoginResponse("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWl" +
                "uIiwibmJmIjoxNjI2ODI0NjAwLCJleHAiOjE2Mjc0Mjk0MDAsImlhdCI6MTYyNjgyNDYwMCwiaXNzIjoiUmVrbGFtYXRpb25B" +
                "UElJc3N1ZXIiLCJhdWQiOiJSZWtsYW1hdGlvbkFQSUF1ZGllbmNlIn0.DxBCl8jU8N41ZKZSz8Fg3UNJSh_zxXaaJ_TuGviP6BQ");
            return response;
        }
    }
}
