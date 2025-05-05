using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;

namespace VetApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class VaccineController(IUnitOfWork unitOfWork) : ControllerBase {
    [HttpPost]
    [Authorize(Roles = nameof(Veterinarian))]
    public async Task<IActionResult> AddVaccine([FromBody] VaccineDto vaccineDto) {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Convert DTO to entity
        var vaccine = VaccineDto.ToVaccine(vaccineDto);

        await unitOfWork.Vaccine.AddAsync(vaccine);
        await unitOfWork.SaveAsync();

        // Return the DTO with updated Id
        return Ok(VaccineDto.FromVaccine(vaccine));
    }

    // multiple vaccines

    [HttpGet("{vetId}")]
    public async Task<IActionResult> GetVaccines(int vetId) {
        var vaccines = await unitOfWork.Vaccine.GetAll(v => v.VetId == vetId);

        return Ok(vaccines.Select(
            VaccineDto.FromVaccine
        ));
    }
}