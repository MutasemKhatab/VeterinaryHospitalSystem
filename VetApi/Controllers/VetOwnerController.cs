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
        [HttpGet]// create another one with vets 
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
    }
}