using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace auth.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpPost("auth")]
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

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username == "streamer" && request.Password == "securepass") // hard-coded credentials for demo purposes
            {
                var guid = Guid.NewGuid();
                var utcNow = DateTime.UtcNow;
                var exp = utcNow.AddHours(2); // token expiration -hard-coded for demo purposes

                // JWT creation
                var claims = new[]
                {
                    new Claim("sub", request.Username ?? ""),
                    new Claim("stream_id", guid.ToString()),
                    new Claim("iat", ((DateTimeOffset)utcNow).ToUnixTimeSeconds().ToString()),
                    new Claim("exp", ((DateTimeOffset)exp).ToUnixTimeSeconds().ToString()),
                    new Claim("role", "test-taker") // hard-coded for demo purposes
                };
                
                string passphrase = "ThisIsASuperSecretKeyThatIsAtLeast32Bytes"; // hard-coded for demo purposes
                byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(passphrase);
                var key = new SymmetricSecurityKey(keyBytes);

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                    claims: claims,
                    signingCredentials: creds
                );
                var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new { streampath = guid.ToString(), jwt, datetime = utcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") });
            }
            
            return Unauthorized();
        }
    }
}
