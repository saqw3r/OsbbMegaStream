using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Authenticate([FromBody] AuthRequest request)
        {
            // Log all incoming data for debugging
            Console.WriteLine($"User: {request.User}, Password: {request.Password}, Path: {request.Path}, Protocol: {request.Protocol}, Query: {request.Query}");

            // Example: Validate JWT token
            if (!string.IsNullOrEmpty(request.Query) && request.Query.Contains("jwt="))
            {
               var token = ExtractJwtFromQuery(request.Query);
               if (ValidateJwt(token))
                   return Ok(); // Authorized
            }

            // Example: Validate username/password
            if (request.User == "streamer" && request.Password == "securepass")
               return Ok(); // Authorized

            return Unauthorized(); // Not authorized
        }

        private string ExtractJwtFromQuery(string query)
        {
            var match = Regex.Match(query, @"jwt=([^&]+)");
            return match.Success ? match.Groups[1].Value : null;
        }

        private bool ValidateJwt(string token)
        {
            // Use JWT library to validate token signature, expiration, etc.
            return true; // Stubbed for example
        }
    }
}
