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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VetOwner>> Get(int id) {
            var vetOwner = await unitOfWork.VetOwner.Get(owner => owner.Id.Equals(id));
            if (vetOwner == null) {
                return NotFound();
            }

            return vetOwner;
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