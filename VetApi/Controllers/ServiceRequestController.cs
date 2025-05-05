using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;
using Vet.Models.DTOs;

namespace VetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ServiceRequestController(IUnitOfWork unitOfWork) : ControllerBase {
    [HttpGet]
    public async Task<IActionResult> GetAll() {
        var serviceRequests = await unitOfWork.ServiceRequest.GetAll();
        return Ok(serviceRequests);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) {
        var serviceRequest = await unitOfWork.ServiceRequest.Get(s => s.Id == id);
        if (serviceRequest == null) {
            return NotFound();
        }

        return Ok(serviceRequest);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ServiceRequestDto serviceRequest) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        var newServiceRequest = serviceRequest.ToServiceRequest();
        await unitOfWork.ServiceRequest.AddAsync(newServiceRequest);
        await unitOfWork.SaveAsync();

        return CreatedAtAction(nameof(Get), new { id = newServiceRequest.Id }, newServiceRequest);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ServiceRequest serviceRequest) {
        if (id != serviceRequest.Id) {
            return BadRequest();
        }

        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        var existingRequest = await unitOfWork.ServiceRequest.Get(s => s.Id == id);
        if (existingRequest == null) {
            return NotFound();
        }

        unitOfWork.ServiceRequest.Update(serviceRequest);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpPut("status/{id:int}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status) {
        var serviceRequest = await unitOfWork.ServiceRequest.Get(s => s.Id == id);
        if (serviceRequest == null) {
            return NotFound();
        }

        serviceRequest.Status = status;
        if (status == "Completed") {
            serviceRequest.CompletionDate = DateTime.UtcNow;
        }

        unitOfWork.ServiceRequest.Update(serviceRequest);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        var serviceRequest = await unitOfWork.ServiceRequest.Get(s => s.Id == id);
        if (serviceRequest == null) {
            return NotFound();
        }

        unitOfWork.ServiceRequest.Remove(serviceRequest);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpGet("owner/{ownerId}")]
    public async Task<IActionResult> GetByOwner(string ownerId) {
        var serviceRequests = await unitOfWork.ServiceRequest.GetAll(s => s.VetOwnerId == ownerId);
        return Ok(serviceRequests);
    }
}