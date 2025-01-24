using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vet.Models;

namespace VetApi.Controllers.Authentication
{
    [Route("api/auth/[controller]")]
    [ApiController]
    public class RegisterController(
        UserManager<VetOwner> userManager)
        : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterBody model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var user = new VetOwner {Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                ProfilePicUrl = model.ProfilePicUrl,
                UserName = model.Email};
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "User registered successfully" });
        }
    }
}

public record RegisterBody
{
    public string Email { get; set; }
    public string Password { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string? Address { get; set; }
    
    public string? ProfilePicUrl { get; set; }
}
