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
        public IActionResult Authenticate([FromForm] string request)
        {
            //// Example: Validate JWT token
            //if (!string.IsNullOrEmpty(request.Query) && request.Query.Contains("jwt="))
            //{
            //    var token = ExtractJwtFromQuery(request.Query);
            //    if (ValidateJwt(token))
            //        return Ok(); // Authorized
            //}

            //// Example: Validate username/password
            //if (request.User == "streamer" && request.Password == "securepass")
            //    return Ok(); // Authorized

            return Ok();

            //return Unauthorized(); // Not authorized
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

    public class AuthRequest
    {
        //public string Ip { get; set; }
        //public string User { get; set; }
        //public string Password { get; set; }
        //public string Action { get; set; }
        //public string Path { get; set; }
        //public string Protocol { get; set; }
        //public string Id { get; set; }
        //public string Query { get; set; }
        //public string traceId { get; set; }
    }
}
