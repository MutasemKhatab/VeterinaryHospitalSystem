using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vet.DataAccess.Data;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VetApi.Controllers {
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class VetOwnerController(IUnitOfWork unitOfWork) : ControllerBase {
        [HttpGet] // create another one with vets 
        public async Task<IEnumerable<VetOwner>> Get() {
            return await unitOfWork.VetOwner.GetAll();
        }

        [HttpGet("current")]
        public async Task<ActionResult<VetOwnerDto>> GetCurrentVetOwner() {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) {
                return Unauthorized();
            }

            var vetOwner = await unitOfWork.VetOwner.Get(owner => owner.Id.Equals(userId));
            if (vetOwner == null) {
                return NotFound(userId);
            }

            return VetOwnerDto.FromVetOwner(vetOwner);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VetOwner>> Get(int id) {
            var vetOwner = await unitOfWork.VetOwner.Get(owner => owner.Id.Equals(id));
            if (vetOwner == null) {
                return NotFound();
            }

            return vetOwner;
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] VetOwnerDto? vetOwnerDto) {
            if (vetOwnerDto == null) {
                return BadRequest("Invalid data.");
            }

            var vetOwner = await unitOfWork.VetOwner.Get(v => v.Id == vetOwnerDto.Id);
            if (vetOwner == null) {
                return NotFound("VetOwner not found.");
            }

            vetOwner.FirstName = vetOwnerDto.FirstName;
            vetOwner.LastName = vetOwnerDto.LastName;
            vetOwner.Email = vetOwnerDto.Email;
            vetOwner.PhoneNumber = vetOwnerDto.PhoneNumber;
            vetOwner.Address = vetOwnerDto.Address;
            vetOwner.ProfilePicUrl = vetOwnerDto.ProfilePicUrl;

            unitOfWork.VetOwner.Update(vetOwner);
            await unitOfWork.SaveAsync();

            return Ok("Profile updated successfully.");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) {
            var vetOwner = await unitOfWork.VetOwner.Get(owner => owner.Id.Equals(id));
            if (vetOwner == null) {
                return NotFound();
            }

            unitOfWork.VetOwner.Remove(vetOwner);
            await unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}