using MediatR;
using HR.Application.Interfaces;
using HR.Domain.Entities;

namespace HR.Application.Features.Employees.Commands.RegisterEmployee;
public class RegisterEmployeeHandler : IRequestHandler<RegisterEmployeeCommand, Guid>
{
    private readonly IUnitOfWork _uow;

    public RegisterEmployeeHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Guid> Handle(RegisterEmployeeCommand request, CancellationToken cancellationToken)
    {
        var existing = await _uow.Employees.GetByEmailAsync(request.Email);
        if (existing is not null)
            throw new InvalidOperationException($"Employee with email {request.Email} already exists.");

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Department = request.Department,
            Designation = request.Designation,
            ReportingManagerId = request.ReportingManagerId,
            JoiningDate = request.JoiningDate
        };

        await _uow.Employees.AddAsync(employee);
        await _uow.SaveChangesAsync();

        return employee.Id;
    }
}