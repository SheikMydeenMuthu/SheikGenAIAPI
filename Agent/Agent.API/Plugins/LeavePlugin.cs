using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace Agent.API.Plugins;

public class LeavePlugin
{
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public LeavePlugin(HttpClient httpClient, string token)
    {
        _httpClient = httpClient;
        _token = token;
    }

    [KernelFunction("apply_leave")]
    [Description("Apply leave for an employee")]
    public async Task<string> ApplyLeaveAsync(
        [Description("Employee ID")] string employeeId,
        [Description("Leave type: 0=Annual, 1=Sick, 2=Casual")] int leaveType,
        [Description("Start date yyyy-MM-dd")] string startDate,
        [Description("End date yyyy-MM-dd")] string endDate,
        [Description("Reason for leave")] string reason)
    {
        var payload = new
        {
            employeeId = Guid.Parse(employeeId),
            leaveType,
            startDate = DateTime.Parse(startDate),
            endDate = DateTime.Parse(endDate),
            reason
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = 
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
        var response = await _httpClient.PostAsync("api/v1/leaverequests", content);

        return response.IsSuccessStatusCode
            ? "Leave applied successfully."
            : $"Failed: {await response.Content.ReadAsStringAsync()}";
    }
}