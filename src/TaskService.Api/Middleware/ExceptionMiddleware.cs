using System.Net;
using System.Text.Json;

namespace TaskService.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Необработанное исключение при обработке запроса {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var body = JsonSerializer.Serialize(new
            {
                error = "Внутренняя ошибка сервера.",
                detail = ex.Message
            });

            await context.Response.WriteAsync(body);
        }
    }
}
