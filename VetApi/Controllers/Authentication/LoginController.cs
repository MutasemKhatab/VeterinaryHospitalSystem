
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Vet.Models;
using VetApi.Utils;

namespace VetApi.Controllers.Authentication
{
    [Route("api/auth/[controller]")]
    [ApiController]
    public class LoginController(
        SignInManager<VetOwner> signInManager,
        UserManager<VetOwner> userManager,
        IOptions<JwtSettings> jwtSettings)
        : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginBody model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            
            if (!result.Succeeded)
                return Unauthorized("Invalid email or password");

            var user = await userManager.FindByEmailAsync(model.Email);
            var token = new TokenGenerator(jwtSettings).GenerateToken(user);

            return Ok(new { Token = token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok(new { Message = "User logged out successfully" });
        }
    }
}

public record LoginBody
{
    public string Email { get; set; }
    public string Password { get; set; }
}