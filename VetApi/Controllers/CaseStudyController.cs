using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;

namespace VetApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(Roles = nameof(Veterinarian))]
public class CaseStudyController(IUnitOfWork unitOfWork) : Controller {
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CaseStudy>>> GetAll() {
        return Ok(await unitOfWork.CaseStudy.GetAll());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CaseStudy>> GetById(int id) {
        var caseStudy = await unitOfWork.CaseStudy.Get(
            filter: c => id == c.Id
            , include: "Veterinarian,VetOwner,Vet"
        );
        if (caseStudy == null) {
            return NotFound();
        }

        return Ok(caseStudy);
    }

    [HttpGet("veterinarian/{id}")]
    public async Task<ActionResult<IEnumerable<CaseStudy>>> GetByVeterinarianId(string id) {
        var veterinarian = await unitOfWork.Veterinarian.Get(
            filter: v => v.Id == id);
        if (veterinarian == null) {
            return NotFound();
        }

        var caseStudies = await unitOfWork.CaseStudy.GetAll(
            filter: c => c.VeterinarianId == id,
            include: "Vet"
        );

        return Ok(caseStudies);
    }

    [HttpPost]
    public async Task<ActionResult<CaseStudy>> Create(CaseStudy caseStudy) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        var validationResult = await ValidateCaseStudyRequest(caseStudy);
        if (validationResult != null) {
            return validationResult;
        }

        await unitOfWork.CaseStudy.AddAsync(caseStudy);
        await unitOfWork.SaveAsync();
        return CreatedAtAction(nameof(GetById), new { id = caseStudy.Id }, caseStudy);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CaseStudy>> Update(int id, CaseStudy caseStudy) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        if (id != caseStudy.Id) {
            return BadRequest("Id mismatch");
        }

        var existingCaseStudy = await unitOfWork.CaseStudy.Get(filter: c => c.Id == id, tracked: false);
        if (existingCaseStudy == null) {
            return NotFound();
        }

        var validationResult = await ValidateCaseStudyRequest(caseStudy);
        if (validationResult != null) {
            return validationResult;
        }

        unitOfWork.CaseStudy.Update(caseStudy);
        await unitOfWork.SaveAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        var caseStudy = await unitOfWork.CaseStudy.Get(filter: c => c.Id == id);
        if (caseStudy == null) {
            return NotFound();
        }

        unitOfWork.CaseStudy.Remove(caseStudy);
        await unitOfWork.SaveAsync();
        return NoContent();
    }

    private async Task<ActionResult?> ValidateCaseStudyRequest(CaseStudy caseStudy) {
        // Validate user identity
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) {
            return Unauthorized("User not found.");
        }

        if (caseStudy.VeterinarianId != userId) {
            return BadRequest("Veterinarian ID does not match the authenticated user.");
        }

        // Validate VetOwner exists
        if (caseStudy.VetOwnerId.IsNullOrEmpty()) {
            return BadRequest("Vet owner ID is required.");
        }

        var vetOwner = await unitOfWork.VetOwner.Get(
            filter: v => v.Id == caseStudy.VetOwnerId,
            include: "Vets");

        if (vetOwner == null) {
            return BadRequest($"Vet owner with ID {caseStudy.VetOwnerId} not found.");
        }

        // Validate Vet exists and belongs to VetOwner
        if (caseStudy.VetId == 0) {
            return BadRequest("Vet ID is required.");
        }

        var vet = vetOwner.Vets?.FirstOrDefault(v => v.Id == caseStudy.VetId);
        if (vet == null) {
            return BadRequest($"Vet with ID {caseStudy.VetId} not found or doesn't belong to the specified owner.");
        }

        return null;
    }
}