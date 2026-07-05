using MediatR;

namespace HR.Application.Features.Employees.Commands.RegisterEmployee;
public record RegisterEmployeeCommand(
    Guid? Id,
    string FirstName,
    string LastName,
    string Email,
    string Department,
    string Designation,
    Guid? ReportingManagerId,
    DateTime JoiningDate) : IRequest<Guid>;