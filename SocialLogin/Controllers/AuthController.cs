using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLogin.Database;
using SocialLogin.Database.Model;
using SocialLogin.ExternalIdP.Google;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SocialLogin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleAsync(AppDbContext db, [FromBody] GoogleAuthRequest request)
        {
            var context = this.HttpContext;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(request.Token);

            // Extract user information
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { success = false, message = "Invalid token" });
            }

            // Find or create the user in the database
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                user = new User { Email = email, Name = name };
                db.Users.Add(user);
                await db.SaveChangesAsync();
            }

            // Sign in the user
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(new { success = true });
        }
    }
}
