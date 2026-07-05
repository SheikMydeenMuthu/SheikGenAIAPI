namespace Agent.API.Services;

public class EndpointService : IEndpointService
{
    private readonly IConfiguration _config;
    public EndpointService(IConfiguration config) => _config = config;
    public Uri GetEndPoint(string provider)
    {
        var url = _config[$"AIProviders:{provider.ToLower()}"]
       ?? throw new NotSupportedException($"Provider {provider} not supported");
        return new Uri(url);
    }
}