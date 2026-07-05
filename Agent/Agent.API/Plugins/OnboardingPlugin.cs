using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace Agent.API.Plugins;

public class OnboardingPlugin
{
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public OnboardingPlugin(HttpClient httpClient, string token)
    {
        _httpClient = httpClient;
        _token = token;
    }

    [KernelFunction("get_onboarding_tasks")]
    [Description("Get onboarding tasks for an employee")]
    public async Task<string> GetOnboardingTasksAsync(
        [Description("Employee ID")] string employeeId)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        var response = await _httpClient.GetAsync($"api/v1/onboardingtasks/{employeeId}");
        return response.IsSuccessStatusCode
            ? await response.Content.ReadAsStringAsync()
            : $"Failed: {await response.Content.ReadAsStringAsync()}";
    }

    [KernelFunction("complete_onboarding_task")]
    [Description("Complete a specific onboarding task")]
    public async Task<string> CompleteOnboardingTaskAsync(
        [Description("Task ID")] string taskId,
        [Description("Employee ID")] string employeeId)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        var payload = new { taskId = Guid.Parse(taskId), employeeId = Guid.Parse(employeeId) };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync("api/v1/onboardingtasks/complete", content);

        return response.IsSuccessStatusCode
            ? "Task completed successfully."
            : $"Failed: {await response.Content.ReadAsStringAsync()}";
    }
}