using Agent.API.Model;
using Agent.API.Plugins;
using Agent.API.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Agent.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AgentController : ControllerBase
{
    private LeavePlugin _leavePlugin;
    private OnboardingPlugin _onboardingPlugin;
    private readonly IHttpClientFactory _factory;
    private readonly IEndpointService _endpointService;
   private static readonly Dictionary<Guid, ChatHistory> _sessions = new();
    public AgentController(IHttpClientFactory factory, IEndpointService endpointService)
    {
        _factory = factory;
        _endpointService = endpointService;
    }


    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        try
        {

            var token = HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") ?? string.Empty;

            var kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(
                modelId: request.ModelId,
                apiKey: request.ApiKey,
                endpoint: _endpointService.GetEndPoint(request.Provider))
            .Build();

            _leavePlugin = new LeavePlugin(_factory.CreateClient("HRClient"), token);

            _onboardingPlugin = new OnboardingPlugin(_factory.CreateClient("HRClient"), token);

            kernel.ImportPluginFromObject(_leavePlugin, "LeavePlugin");
            kernel.ImportPluginFromObject(_onboardingPlugin, "OnboardingPlugin");

            // var leaveResult = await kernel.InvokeAsync("LeavePlugin", "apply_leave", new KernelArguments
            // {
            //     ["employeeId"] = request.EmployeeId.ToString(),
            //     ["leaveType"] = 0,
            //     ["startDate"] = request.StartDate,
            //     ["endDate"] = request.EndDate,
            //     ["reason"] = request.Message
            // });

            // var onboardingResult = await kernel.InvokeAsync("OnboardingPlugin", "get_onboarding_tasks", new KernelArguments
            // {
            //     ["employeeId"] = request.EmployeeId.ToString()
            // });

            if (!_sessions.TryGetValue(request.EmployeeId, out var history))
            {
                history = new ChatHistory();
                _sessions[request.EmployeeId] = history;
            }

            var settings = new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            var chatService = kernel.GetRequiredService<IChatCompletionService>();
            var prompt = $"EmployeeId: {request.EmployeeId}. {request.Message}";
            history.AddUserMessage(prompt);

            var response = await chatService.GetChatMessageContentAsync(history, settings, kernel, cancellationToken);
            history.AddAssistantMessage(response.Content!);

            return Ok(new { reply = response.Content });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(408, new { error = "Request timed out." });
        }
    }
}