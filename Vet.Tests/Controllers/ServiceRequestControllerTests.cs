using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;
using Vet.Models.DTOs;
using VetApi.Controllers;
using Xunit;

namespace Vet.Tests.Controllers;

public class ServiceRequestControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ServiceRequestController _controller;

    public ServiceRequestControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _controller = new ServiceRequestController(_mockUnitOfWork.Object);
    }

    #region GetAllTests

    [Fact]
    public async Task GetAll_Should_Return_OkWithEmptyList()
    {
        // Arrange
        _mockUnitOfWork.Setup(work => work.ServiceRequest.GetAll(null, null))
            .ReturnsAsync([]);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var serviceRequests = Assert.IsAssignableFrom<IEnumerable<ServiceRequest>>(okResult.Value);
        Assert.NotNull(serviceRequests);
        Assert.Empty(serviceRequests);
    }

    [Fact]
    public async Task GetAll_Should_Return_OkWithServiceRequests()
    {
        // Arrange
        var serviceRequests = new List<ServiceRequest>
        {
            new ServiceRequest { Id = 1, VetOwnerId = "owner1", Title = "Request 1", Description = "Desc1" },
            new ServiceRequest { Id = 2, VetOwnerId = "owner2", Title = "Request 2", Description = "Desc2" }
        };

        _mockUnitOfWork.Setup(work => work.ServiceRequest.GetAll(null, null))
            .ReturnsAsync(serviceRequests);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRequests = Assert.IsAssignableFrom<IEnumerable<ServiceRequest>>(okResult.Value);
        Assert.NotNull(returnedRequests);
        Assert.Equal(2, returnedRequests.Count());
        Assert.Contains(returnedRequests, r => r.Id == 1);
        Assert.Contains(returnedRequests, r => r.Id == 2);
    }

    #endregion

    #region GetTests

    [Fact]
    public async Task Get_Should_Return_NotFound_When_ServiceRequestDoesNotExist()
    {
        // Arrange
        _mockUnitOfWork.Setup(work => work.ServiceRequest.Get(It.IsAny<Expression<Func<ServiceRequest, bool>>>(), null, true))
            .ReturnsAsync(null as ServiceRequest);

        // Act
        var result = await _controller.Get(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Get_Should_Return_OkWithServiceRequest()
    {
        // Arrange
        var serviceRequest = new ServiceRequest
        {
            Id = 1,
            VetOwnerId = "owner1",
            Title = "Test Request",
            Description = "Test Description",
            Status = "Pending"
        };

        _mockUnitOfWork.Setup(work => work.ServiceRequest.Get(It.IsAny<Expression<Func<ServiceRequest, bool>>>(), null, true))
            .ReturnsAsync(serviceRequest);

        // Act
        var result = await _controller.Get(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRequest = Assert.IsType<ServiceRequest>(okResult.Value);
        Assert.Equal(1, returnedRequest.Id);
        Assert.Equal("owner1", returnedRequest.VetOwnerId);
        Assert.Equal("Test Request", returnedRequest.Title);
        Assert.Equal("Test Description", returnedRequest.Description);
        Assert.Equal("Pending", returnedRequest.Status);
    }

    #endregion

    #region CreateTests

    [Fact]
    public async Task Create_Should_Return_BadRequest_When_ModelStateIsInvalid()
    {
        // Arrange
        var serviceRequestDto = new ServiceRequestDto();
        _controller.ModelState.AddModelError("Title", "Title is required");

        // Act
        var result = await _controller.Create(serviceRequestDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
        _mockUnitOfWork.Verify(u => u.ServiceRequest.AddAsync(It.IsAny<ServiceRequest>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveAsync("auth"), Times.Never);
    }

    [Fact]
    public async Task Create_Should_Return_CreatedAtAction_When_Successful()
    {
        // Arrange
        var serviceRequestDto = new ServiceRequestDto
        {
            Title = "New Request",
            Description = "Test Description",
            VetOwnerId = "owner1",
            Status = "Pending",
            RequestDate = DateTime.UtcNow
        };

        _mockUnitOfWork.Setup(u => u.ServiceRequest.AddAsync(It.IsAny<ServiceRequest>()))
            .Returns(Task.CompletedTask)
            .Callback<ServiceRequest>(request => request.Id = 1); // Simulate auto-assigned Id

        _mockUnitOfWork.Setup(u => u.SaveAsync("auth")).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(serviceRequestDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ServiceRequestController.Get), createdAtActionResult.ActionName);
        Assert.Equal(1, createdAtActionResult.RouteValues!["id"]);
        
        var returnedRequest = Assert.IsType<ServiceRequest>(createdAtActionResult.Value);
        Assert.Equal("New Request", returnedRequest.Title);
        Assert.Equal("Test Description", returnedRequest.Description);
        Assert.Equal("owner1", returnedRequest.VetOwnerId);
        Assert.Equal("Pending", returnedRequest.Status);
        
        _mockUnitOfWork.Verify(u => u.ServiceRequest.AddAsync(It.IsAny<ServiceRequest>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveAsync("auth"), Times.Once);
    }

    #endregion

    #region UpdateTests

    [Fact]
    public async Task Update_Should_Return_BadRequest_When_IdsDoNotMatch()
    {
        // Arrange
        var serviceRequest = new ServiceRequest { Id = 2 };

        // Act
        var result = await _controller.Update(1, serviceRequest);

        // Assert
        Assert.IsType<BadRequestResult>(result);
        _mockUnitOfWork.Verify(u => u.ServiceRequest.Get(It.IsAny<Expression<Func<ServiceRequest, bool>>>(), null, true), Times.Never);
        _mockUnitOfWork.Verify(u => u.ServiceRequest.Update(It.IsAny<ServiceRequest>()), Times.Never);
    }

    [Fact]
    public async Task Update_Should_Return_BadRequest_When_ModelStateIsInvalid()
    {
        // Arrange
        var serviceRequest = new ServiceRequest { Id = 1 };
        _controller.ModelState.AddModelError("Title", "Title is required");

        // Act
        var result = await _controller.Update(1, serviceRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
        _mockUnitOfWork.Verify(u => u.ServiceRequest.Update(It.IsAny<ServiceRequest>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveAsync("auth"), Times.Never);
    }

    [Fact]
    public async Task Update_Should_Return_NotFound_When_ServiceRequestDoesNotExist()
    {
        // Arrange
        var serviceRequest = new ServiceRequest { 
            Id = 1,
            Title = "Updated Request",
            Description = "Updated Description"
        };
        
        _mockUnitOfWork.Setup(u => u.ServiceRequest.Get(It.IsAny<Expression<Func<ServiceRequest, bool>>>(), null, true))
            .ReturnsAsync(null as ServiceRequest);

        // Act
        var result = await _controller.Update(1, serviceRequest);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockUnitOfWork.Verify(u => u.ServiceRequest.Update(It.IsAny<ServiceRequest>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveAsync("auth"), Times.Never);
    }

    [Fact]
    public async Task Update_Should_Return_NoContent_When_UpdateIsSuccessful()
    {
        // Arrange
        var serviceRequest = new ServiceRequest
        {
            Id = 1,
            VetOwnerId = "owner1",
            Title = "Updated Request",
            Description = "Updated Description",
            Status = "In Progress"
        };

        var existingRequest = new ServiceRequest { Id = 1 };

        _mockUnitOfWork.Setup(u => u.ServiceRequest.Get(It.IsAny<Expression<Func<ServiceRequest, bool>>>(), null, true))
            .ReturnsAsync(existingRequest);
        _mockUnitOfWork.Setup(u => u.SaveAsync("auth")).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(1, serviceRequest);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockUnitOfWork.Verify(u => u.ServiceRequest.Update(serviceRequest), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveAsync("auth"), Times.Once);
    }

    #endregion

    #region UpdateStatusTests

    [Fact]
    public async Task UpdateStatus_Should_Return_NotFound_When_ServiceRequestDoesNotExist()
    {
        // Arrange
        _mockUnitOfWork.Setup(u => u.ServiceRequest.Get(It.IsAny<Expression<Func<ServiceRequest, bool>>>(), null, true))
            .ReturnsAsync(null as ServiceRequest);

        // Act
        var result = await _controller.UpdateStatus(1, "In Progress");

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockUnitOfWork.Verify(u => u.ServiceRequest.Update(It.IsAny<ServiceRequest>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveAsync("auth"), Times.Never);
    }

    [Fact]
    public async Task UpdateStatus_Should_Return_NoContent_And_Update_Status_Only()
    {
        // Arrange
        var existingRequest = new ServiceRequest
        {
            Id = 1,
            Title = "Test Request",
            Status = "Pending",
            CompletionDate = null
        };

        _mockUnitOfWork.Setup(u => u.ServiceRequest.Get(It.IsAny<Expression<Func<ServiceRequest, bool>>>(), null, true))
            .ReturnsAsync(existingRequest);
        _mockUnitOfWork.Setup(u => u.SaveAsync("auth")).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateStatus(1, "In Progress");

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal("In Progress", existingRequest.Status);
        Assert.Null(existingRequest.CompletionDate);
        _mockUnitOfWork.Verify(u => u.ServiceRequest.Update(existingRequest), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveAsync("auth"), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_Should_Set_CompletionDate_When_StatusIsCompleted()
    {
        // Arrange
        var existingRequest = new ServiceRequest
        {
            Id = 1,
            Title = "Test Request",
            Status = "In Progress",
            CompletionDate = null
        };

        var beforeTest = DateTime.UtcNow;

        _mockUnitOfWork.Setup(u => u.ServiceRequest.Get(It.IsAny<Expression<Func<ServiceRequest, bool>>>(), null, true))
            .ReturnsAsync(existingRequest);
        _mockUnitOfWork.Setup(u => u.SaveAsync("auth")).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateStatus(1, "Completed");
        var afterTest = DateTime.UtcNow;

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal("Completed", existingRequest.Status);
        Assert.NotNull(existingRequest.CompletionDate);
        
        // Verify completion date was set within the expected timeframe
        Assert.True(existingRequest.CompletionDate >= beforeTest);
        Assert.True(existingRequest.CompletionDate <= afterTest);
        
        _mockUnitOfWork.Verify(u => u.ServiceRequest.Update(existingRequest), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveAsync("auth"), Times.Once);
    }

    #endregion

    #region DeleteTests

    [Fact]
    public async Task Delete_Should_Return_NotFound_When_ServiceRequestDoesNotExist()
    {
        // Arrange
        _mockUnitOfWork.Setup(u => u.ServiceRequest.Get(It.IsAny<Expression<Func<ServiceRequest, bool>>>(), null, true))
            .ReturnsAsync(null as ServiceRequest);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockUnitOfWork.Verify(u => u.ServiceRequest.Remove(It.IsAny<ServiceRequest>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveAsync("auth"), Times.Never);
    }

    [Fact]
    public async Task Delete_Should_Return_NoContent_When_DeleteIsSuccessful()
    {
        // Arrange
        var existingRequest = new ServiceRequest { 
            Id = 1, 
            Title = "Test Request",
            Description = "Test Description"
        };

        _mockUnitOfWork.Setup(u => u.ServiceRequest.Get(It.IsAny<Expression<Func<ServiceRequest, bool>>>(), null, true))
            .ReturnsAsync(existingRequest);
        _mockUnitOfWork.Setup(u => u.SaveAsync("auth")).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockUnitOfWork.Verify(u => u.ServiceRequest.Remove(existingRequest), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveAsync("auth"), Times.Once);
    }

    #endregion

    #region GetByOwnerTests

    [Fact]
    public async Task GetByOwner_Should_Return_OkWithEmptyList_When_NoRequestsExist()
    {
        // Arrange
        _mockUnitOfWork.Setup(work => work.ServiceRequest.GetAll(It.IsAny<Expression<Func<ServiceRequest, bool>>>(), null))
            .ReturnsAsync([]);

        // Act
        var result = await _controller.GetByOwner("owner1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var serviceRequests = Assert.IsAssignableFrom<IEnumerable<ServiceRequest>>(okResult.Value);
        Assert.NotNull(serviceRequests);
        Assert.Empty(serviceRequests);
    }

    [Fact]
    public async Task GetByOwner_Should_Return_OkWithServiceRequests()
    {
        // Arrange
        var ownerId = "owner1";
        var serviceRequests = new List<ServiceRequest>
        {
            new ServiceRequest { Id = 1, VetOwnerId = ownerId, Title = "Request 1", Description = "Desc1" },
            new ServiceRequest { Id = 2, VetOwnerId = ownerId, Title = "Request 2", Description = "Desc2" }
        };

        _mockUnitOfWork.Setup(work => work.ServiceRequest.GetAll(
            It.Is<Expression<Func<ServiceRequest, bool>>>(expr => expr.Compile().Invoke(new ServiceRequest { VetOwnerId = ownerId })),
            null))
            .ReturnsAsync(serviceRequests);

        // Act
        var result = await _controller.GetByOwner(ownerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRequests = Assert.IsAssignableFrom<IEnumerable<ServiceRequest>>(okResult.Value);
        Assert.NotNull(returnedRequests);
        Assert.Equal(2, returnedRequests.Count());
        Assert.All(returnedRequests, r => Assert.Equal(ownerId, r.VetOwnerId));
    }

    [Fact]
    public async Task GetByOwner_Should_Return_OkWithEmptyList_When_OwnerHasNoRequests()
    {
        // Arrange
        var nonExistentOwnerId = "nonexistent";
        
        _mockUnitOfWork.Setup(work => work.ServiceRequest.GetAll(
            It.Is<Expression<Func<ServiceRequest, bool>>>(expr => expr.Compile().Invoke(new ServiceRequest { VetOwnerId = nonExistentOwnerId })),
            null))
            .ReturnsAsync([]);

        // Act
        var result = await _controller.GetByOwner(nonExistentOwnerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRequests = Assert.IsAssignableFrom<IEnumerable<ServiceRequest>>(okResult.Value);
        Assert.NotNull(returnedRequests);
        Assert.Empty(returnedRequests);
    }

    #endregion
}
