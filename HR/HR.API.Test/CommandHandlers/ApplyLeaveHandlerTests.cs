using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using HR.Domain.Entities;
using HR.Domain.Enums;
using HR.Application.Interfaces;
using HR.Application.Common.Exceptions;
using HR.Application.Features.LeaveRequests.Commands.ApplyLeave;
using HR.Domain.Entities;

namespace HR.API.Test.CommandHandlers;

public class ApplyLeaveHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_CreatesLeaveRequestAndReturnsId()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        var mockEmployeeRepo = new Mock<IEmployeeRepository>();
        mockEmployeeRepo.Setup(r => r.GetByIdAsync(employeeId))
                        .ReturnsAsync(new Employee { Id = employeeId, FirstName = "John", LastName = "Doe", Email = "john.doe@test.com" });

        var mockLeaveRepo = new Mock<ILeaveRequestRepository>();
        mockLeaveRepo.Setup(r => r.AddAsync(It.IsAny<LeaveRequest>()))
                     .Returns(Task.CompletedTask);

        var mockUow = new Mock<IUnitOfWork>();
        mockUow.Setup(u => u.Employees).Returns(mockEmployeeRepo.Object);
        mockUow.Setup(u => u.LeaveRequests).Returns(mockLeaveRepo.Object);
        mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var handler = new ApplyLeaveHandler(mockUow.Object);

        var command = new ApplyLeaveCommand(
            employeeId,
            LeaveType.Casual,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(3),
            "Personal");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        mockLeaveRepo.Verify(r => r.AddAsync(It.IsAny<LeaveRequest>()), Times.Once);
        mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_EmployeeNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        var mockEmployeeRepo = new Mock<IEmployeeRepository>();
        mockEmployeeRepo.Setup(r => r.GetByIdAsync(employeeId))
                        .ReturnsAsync((Employee?)null);

        var mockUow = new Mock<IUnitOfWork>();
        mockUow.Setup(u => u.Employees).Returns(mockEmployeeRepo.Object);

        var handler = new ApplyLeaveHandler(mockUow.Object);

        var command = new ApplyLeaveCommand(
            employeeId,
            LeaveType.Sick,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            "Fever");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(5, 5, 10)]
    [InlineData(-1, 1, 0)]
    public void Add_TwoNumbers_ReturnsSum(int a, int b, int expected)
    {
        var result = a + b;
        Assert.Equal(expected, result);
    }

[Theory]
[InlineData(LeaveType.Casual)]
[InlineData(LeaveType.Sick)]
[InlineData(LeaveType.Annual)]
public async Task Handle_DifferentLeaveTypes_CreatesLeaveRequestSuccessfully(LeaveType leaveType)
{
    // Arrange
    var employeeId = Guid.NewGuid();

    var mockEmployeeRepo = new Mock<IEmployeeRepository>();
    mockEmployeeRepo.Setup(r => r.GetByIdAsync(employeeId))
                    .ReturnsAsync(new Employee { Id = employeeId, FirstName = "John", LastName = "Doe" });

    var mockLeaveRepo = new Mock<ILeaveRequestRepository>();
    mockLeaveRepo.Setup(r => r.AddAsync(It.IsAny<LeaveRequest>()))
                 .Returns(Task.CompletedTask);

    var mockUow = new Mock<IUnitOfWork>();
    mockUow.Setup(u => u.Employees).Returns(mockEmployeeRepo.Object);
    mockUow.Setup(u => u.LeaveRequests).Returns(mockLeaveRepo.Object);
    mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

    var handler = new ApplyLeaveHandler(mockUow.Object);

    var command = new ApplyLeaveCommand(
        employeeId, leaveType, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), "Reason");

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.NotEqual(Guid.Empty, result);
    mockLeaveRepo.Verify(r => r.AddAsync(It.Is<LeaveRequest>(lr => lr.LeaveType == leaveType)), Times.Once);
}
}