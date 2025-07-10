using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using Microsoft.IdentityModel.JsonWebTokens;

namespace auth.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private const string JwtKeyId = "megastream-key";

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        // RSA key for JWT signing
        private static readonly System.Security.Cryptography.RSA Rsa = System.Security.Cryptography.RSA.Create();
        private static readonly RsaSecurityKey RsaKey = new RsaSecurityKey(Rsa)
        {
            KeyId = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JwtKeyId))
        };

        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody] LoginRequest request)
        {
            if (request.Username == "streamer" && request.Password == "securepass")
            {
                var guid = Guid.NewGuid();
                var utcNow = DateTime.UtcNow;
                var exp = utcNow.AddHours(2);

                var permissionsJson = $$"""
                [{
                    "action": "publish",
                    "path": "{{guid}}"
                }]
                """;

                var claims = new[]
                {
                    new Claim("sub", request.Username ?? ""),
                    new Claim("stream_id", guid.ToString()),
                    new Claim("iat", ((DateTimeOffset)utcNow).ToUnixTimeSeconds().ToString()),
                    new Claim("exp", ((DateTimeOffset)exp).ToUnixTimeSeconds().ToString()),
                    new Claim("role", "test-taker"),
                    new Claim("mediamtx_permissions", permissionsJson)
                };

                var singningCredentials = new SigningCredentials(RsaKey, SecurityAlgorithms.RsaSha256);
                var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                    claims: claims,
                    signingCredentials: singningCredentials,
                    notBefore: utcNow,
                    expires: exp
                );
                var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new { streampath = guid.ToString(), jwt, datetime = utcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") });
            }
            return Unauthorized();
        }

        [HttpGet("jwks.json")]
        public IActionResult GetJwks()
        {
            // RsaKey is your same RsaSecurityKey from signing
            var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(RsaKey);

            // It already sets kty, use, kid, alg, n, e correctly
            var jwksJson = JsonSerializer.Serialize(new { keys = new[] { jwk } });
            var set = new JsonWebKeySet(jwksJson);
            return new JsonResult(set);
        }
    }
}
