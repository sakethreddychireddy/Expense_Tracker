using System.Net;
using System.Text.Json;

namespace Expense_Tracker.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Continue pipeline
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var statusCode = ex switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,            // 404
                UnauthorizedAccessException => (int)HttpStatusCode.Forbidden,    // 403
                ApplicationException => (int)HttpStatusCode.BadRequest,          // 400
                _ => (int)HttpStatusCode.InternalServerError                    // 500
            };

            var problemDetails = new
            {
                statusCode,
                message = ex.Message,
                details = ex.InnerException?.Message,
                path = context.Request.Path,
                traceId = context.TraceIdentifier
            };

            _logger.LogError(ex, "An error occurred while processing request {Path}", context.Request.Path);

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
        }
    }
}
