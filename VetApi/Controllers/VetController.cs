using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;

namespace VetApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]

    public class VetController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private async Task<VetOwner?> GetUser()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await unitOfWork.VetOwner.Get(
                owner => owner.Id.Equals(id),
                "Vets"
            );
        }

        // GET: /Vet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vet.Models.Vet>>> GetVets()
        {
            var user = await GetUser();

            if (user == null)
            {
                return NotFound();
            }

            return user.Vets;
        }

        // GET: /Vet/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Vet.Models.Vet>> GetVet(int id)
        {
            var user = await GetUser();
            if (user == null)
                return NotFound();

            var vet = user.Vets.FirstOrDefault(v => v.Id == id);
            if (vet == null)
                return NotFound();

            return vet;
        }

        // POST: /Vet
        [HttpPost]
        public async Task<ActionResult<Vet.Models.Vet>> PostVet(Vet.Models.Vet vet)
        {
            var user = await GetUser();
            if (user == null)
            {
                return NotFound();
            }

            user.Vets.Add(vet);
            await unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetVet), new { id = vet.Id }, vet);
        }


        // PUT: /Vet/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutVet(int id, Vet.Models.Vet vet)
        {
            var user = await GetUser();
            if (user == null || id != vet.Id || vet.OwnerId != user.Id)
            {
                return BadRequest();
            }

            var existingVet = user.Vets.FirstOrDefault(v => v.Id == id);
            if (existingVet == null)
            {
                return NotFound();
            }

            existingVet.Name = vet.Name;
            existingVet.Species = vet.Species;
            existingVet.ProfilePicUrl = vet.ProfilePicUrl;
            existingVet.Gender = vet.Gender;
            existingVet.DateOfBirth = vet.DateOfBirth;
            unitOfWork.VetOwner.UpdateVet(existingVet);
            try
            {
                await unitOfWork.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (user.Vets.All(e => e.Id != id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: /Vet/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVet(int id)
        {
            var user =await GetUser();

            if (user == null)
            {
                return NotFound();
            }

            var vet = user.Vets.FirstOrDefault(v => v.Id == id);
            if (vet == null)
            {
                return NotFound();
            }

            unitOfWork.VetOwner.DeleteVet(vet);
            await unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}