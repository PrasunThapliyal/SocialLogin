using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLogin.Database;
using SocialLogin.Database.Model;
using SocialLogin.ExternalIdP.Google;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;

namespace SocialLogin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(
            ILogger<AuthController> logger,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this._signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        //[HttpPost("google")]
        //public async Task<IActionResult> GoogleAsync(AppDbContext db, [FromBody] GoogleAuthRequest request)
        //{
        //    var context = this.HttpContext;

        //    var handler = new JwtSecurityTokenHandler();
        //    var jwtToken = handler.ReadJwtToken(request.Token);

        //    // Extract user information
        //    var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        //    var name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return BadRequest(new { success = false, message = "Invalid token" });
        //    }

        //    // Find or create the user in the database
        //    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        //    if (user == null)
        //    {
        //        user = new User { Email = email, Name = name };
        //        db.Users.Add(user);
        //        await db.SaveChangesAsync();
        //    }

        //    // Sign in the user
        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //        new Claim(ClaimTypes.Name, user.Name),
        //        new Claim(ClaimTypes.Email, user.Email)
        //    };

        //    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //    var principal = new ClaimsPrincipal(identity);

        //    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        //    return Ok(new { success = true });
        //}


        [HttpPost("google")]
        public async Task<IActionResult> GoogleAsync(
            AppDbContext db,
            [FromBody] GoogleAuthRequest request)
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

            // Find or create the user in Identity
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser { UserName = email, Email = email };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new { success = false, message = "Failed to create user" });
                }
            }

            // Sign in the user
            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok(new { success = true });
        }

    }
}
