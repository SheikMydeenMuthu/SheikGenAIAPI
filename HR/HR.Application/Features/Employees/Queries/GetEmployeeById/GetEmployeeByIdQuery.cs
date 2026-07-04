using MediatR;

namespace HR.Application.Features.Employees.Queries.GetEmployeeById;
public record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeDto>;