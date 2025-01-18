using System.Text;

namespace Web.Infrastructure;

public sealed class LoggingMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate requestDelegate, ILogger<LoggingMiddleware> logger)
    {
        _requestDelegate = requestDelegate;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        LogRequest(context);
        
        await _requestDelegate(context);
        
        LogResponse(context);
    }

    private void LogRequest(HttpContext context)
    {
        var request = context.Request;

        var sb = new StringBuilder();

        sb.AppendLine("Incoming request: ");
        sb.AppendLine($"Request: {request.Method} {request.Path}");
        sb.AppendLine($"HTTP Method: {request.Method}");
        sb.AppendLine($"$Host: P{request.Host}");
        
        _logger.LogInformation(sb.ToString());
    }

    private void LogResponse(HttpContext context)
    {
        var response = context.Response;
        
        var sb = new StringBuilder();
        
        sb.AppendLine("Outgoing response: ");
        sb.AppendLine($"Status code: {response.StatusCode}");
        
        _logger.LogInformation(sb.ToString());
    }
}