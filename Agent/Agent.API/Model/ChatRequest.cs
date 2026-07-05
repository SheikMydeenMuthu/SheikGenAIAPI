namespace Agent.API.Model;
public record ChatRequest(
    string Message,
    Guid EmployeeId,
    string StartDate,
    string EndDate,
    string ModelId,
    string ApiKey,
    string Provider);