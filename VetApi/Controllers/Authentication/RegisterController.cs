using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vet.Models;

namespace VetApi.Controllers.Authentication {
    [Route("api/auth/[controller]")]
    [ApiController]
    public class RegisterController(
        UserManager<ApplicationUser> userManager)
        : ControllerBase {
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto model) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            ApplicationUser user;
            try {
                user = MapUser(model);
            }
            catch (ArgumentException e) {
                return BadRequest(e.Message);
            }

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "User registered successfully" });
        }

        private static ApplicationUser MapUser(RegisterDto model) {
            if (model.IsVetOwner)
                return MapVetOwner(model);
            return MapVeterinarian(model);
        }

        private static VetOwner MapVetOwner(RegisterDto model) {
            return new VetOwner {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                ProfilePicUrl = model.ProfilePicUrl,
                PhoneNumber = model.PhoneNumber
            };
        }

        private static Veterinarian MapVeterinarian(RegisterDto model) {
            if (string.IsNullOrEmpty(model.EmployeeId) || string.IsNullOrEmpty(model.Speciality))
                throw new ArgumentException("EmployeeId and Speciality are required for Veterinarian");
            return new Veterinarian {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                ProfilePicUrl = model.ProfilePicUrl,
                EmployeeId = model.EmployeeId,
                Speciality = model.Speciality,
                PhoneNumber = model.PhoneNumber
            };
        }
    }
}