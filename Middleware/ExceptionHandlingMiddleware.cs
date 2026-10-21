using System.Text.Json;

namespace progressa.Middleware
{
    public class ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ohanterat fel på {Method} {Path}",
                    context.Request.Method, context.Request.Path);

                if (context.Response.HasStarted) throw;

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var body = new
                {
                    error = "Något gick fel.",
                    detail = env.IsDevelopment() ? ex.Message : null
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(body));
            }
        }
    }

}