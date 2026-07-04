using MediatR;
using HR.Application.Features.Employees.Queries.GetEmployeeById;
using HR.Application.Interfaces;
using HR.Domain.Entities;
using HR.Application.Common.Exceptions;

namespace HR.Application.Features.Employees.Queries.GetEmployeeById;
public class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    private readonly IUnitOfWork _uow;

    public GetEmployeeByIdHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<EmployeeDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _uow.Employees.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Employee), request.Id);

        return new EmployeeDto(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.Department,
            employee.Designation,
            employee.JoiningDate,
            employee.IsOnboarded);
    }
}