using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;

namespace VetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VeterinarianController(IUnitOfWork unitOfWork) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<Veterinarian>> Get()
        {
            return await unitOfWork.Veterinarian.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Veterinarian>> Get(string id)
        {
            var veterinarian = await unitOfWork.Veterinarian.Get(vet => vet.Id.Equals(id));
            if (veterinarian == null)
            {
                return NotFound();
            }

            return veterinarian;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var veterinarian = await unitOfWork.Veterinarian.Get(vet => vet.Id.Equals(id));
            if (veterinarian == null)
            {
                return NotFound();
            }

            unitOfWork.Veterinarian.Remove(veterinarian);
            await unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}
