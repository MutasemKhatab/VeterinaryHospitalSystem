using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vet.DataAccess.Data;
using Vet.Models;

namespace VetApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class VetController(AuthDbContext context) : ControllerBase
    {
        private async Task<VetOwner?> GetUser()
        {
            string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await context.VetOwners.Include(
                owner => owner.Vets
                ).FirstOrDefaultAsync(vo => vo.Id == id);
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
        [HttpGet("{id}")]
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
            await context.SaveChangesAsync();

            return CreatedAtAction("GetVet", new { id = vet.Id }, vet);
        }

        
        // PUT: /Vet/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVet(int id, Vet.Models.Vet vet)
        {
            var user = await GetUser();
            if (user == null || id != vet.Id || vet.OwnerId != user.Id)
            {
                return BadRequest(
                new {
                    vet.OwnerId,
                    id,
                    user
                });
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

            context.Entry(existingVet).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVet(int id)
        {
            var user = await GetUser();

            if (user == null)
            {
                return NotFound();
            }

            var vet = user.Vets.FirstOrDefault(v => v.Id == id);
            if (vet == null)
            {
                return NotFound();
            }

            user.Vets.Remove(vet);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}