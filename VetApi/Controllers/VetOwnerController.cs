using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vet.DataAccess.Data;
using Vet.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VetApi.Controllers {
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class VetOwnerController(AuthDbContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VetOwner>>> Get() {
            return await db.VetOwners.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VetOwner>> Get(int id) {
            var vetOwner = await db.VetOwners.FindAsync(id);
            if (vetOwner == null) {
                return NotFound();
            }
            return vetOwner;
        }

        [HttpPost]
        public async Task<ActionResult<VetOwner>> Post([FromBody] VetOwner vetOwner) {
            db.VetOwners.Add(vetOwner);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = vetOwner.Id }, vetOwner);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] VetOwner vetOwner) {
            if (id != vetOwner.Id) {
                return BadRequest();
            }

            db.Entry(vetOwner).State = EntityState.Modified;

            try {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!VetOwnerExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) {
            var vetOwner = await db.VetOwners.FindAsync(id);
            if (vetOwner == null) {
                return NotFound();
            }

            db.VetOwners.Remove(vetOwner);
            await db.SaveChangesAsync();

            return NoContent();
        }

        private bool VetOwnerExists(string id) {
            return db.VetOwners.Any(e => e.Id == id);
        }
    }
}
