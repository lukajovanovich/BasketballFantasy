using BasketballFantasy.API.Common.Exceptions;

namespace BasketballFantasy.API.Middleware
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;

        public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
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
            catch (ApiException exception)
            {
                await WriteErrorAsync(context, exception.StatusCode, exception.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unhandled exception while processing request.");
                await WriteErrorAsync(
                    context,
                    StatusCodes.Status500InternalServerError,
                    "Doslo je do neocekivane greske.");
            }
        }

        private static Task WriteErrorAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsJsonAsync(new
            {
                Message = message
            });
        }
    }
}
