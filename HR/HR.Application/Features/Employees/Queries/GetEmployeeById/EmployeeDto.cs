namespace HR.Application.Features.Employees.Queries.GetEmployeeById;
public record EmployeeDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Department,
    string Designation,
    DateTime JoiningDate,
    bool IsOnboarded);