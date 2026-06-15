using FluentValidation;

namespace Server.Exceptions;

public class GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
{
    private record ApiErrorResponse
    {
        public int StatusCode { get; init; }
        public string Message { get; init; } = "An unexpected error occurred.";
        public string? TraceId { get; init; }
    }

    private record ApiValidationErrorResponse
    {
        public int StatusCode { get; init; } = StatusCodes.Status400BadRequest;
        public string Message { get; init; } = "One or more validation errors occured.";
        public Dictionary<string, string[]> Errors { get; init; } = new();
        public string? TraceId { get; init; }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");

            if (ex is ValidationException validationException)
            {
                await context.Response.WriteAsJsonAsync(new ApiValidationErrorResponse
                {
                    Errors = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        ),
                    TraceId = context.TraceIdentifier
                });
            }
            else
            {
                context.Response.StatusCode = ex switch
                {
                    BadRequestException => StatusCodes.Status400BadRequest,
                    UnauthorizedException or UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    NotFoundException or KeyNotFoundException => StatusCodes.Status404NotFound,
                    ConflictException => StatusCodes.Status409Conflict,
                    InternalServerException => StatusCodes.Status500InternalServerError,
                    _ => StatusCodes.Status500InternalServerError
                };

                await context.Response.WriteAsJsonAsync(new ApiErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.Message,
                    TraceId = context.TraceIdentifier
                });
            }
        }
    }
}