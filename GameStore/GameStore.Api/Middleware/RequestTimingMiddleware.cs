using System.Diagnostics;

namespace GameStore.Api.Middleware;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = new Stopwatch();

        try
        {
            stopwatch.Start();
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation(
                "{RequestMethod} {RequestPath} request took {ElapsedMilliseconds}ms to complete",
                context.Request.Method,
                context.Request.Path,
                elapsedMilliseconds);
        }
    }
}
