namespace SukotSystemApi.Middlewares
{
    // Runs on every request. Makes sure every request has a CorrelationId,
    // puts it on the response header, and pushes it into the logging scope
    // so every log line written while handling this request - from this
    // middleware, from a controller, from a service - carries the same id.
    public class CorrelationIdMiddleware
    {
        //נותן מזהה ייחודי לכל request
        public const string HeaderName = "X-Correlation-Id";

        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Reuse an incoming id if the caller already supplied one
            // (useful if this API is ever called by another service that
            // wants one id across both systems); otherwise mint a new one.
            var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var existing)
                                 && !string.IsNullOrWhiteSpace(existing)
                ? existing.ToString()
                : Guid.NewGuid().ToString();

            // Store it on HttpContext.Items so controllers/services further down
            // the pipeline can read it too, e.g. context.Items["CorrelationId"].
            context.Items["CorrelationId"] = correlationId;

            // Headers can only be set before the response starts writing.
            // OnStarting guarantees this runs at exactly the right moment,
            // even though we register it before we know the final status code.
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationId;
                return Task.CompletedTask;
            });

            // BeginScope attaches CorrelationId to every log written inside this
            // using block, for as long as the request is being processed - this
            // is what "included in the logs" (Part D) actually means in code.
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
            {
                _logger.LogInformation("Incoming request {Method} {Path}", context.Request.Method, context.Request.Path);

                await _next(context);
            }
        }
    }

    // Extension method so Program.cs reads as app.UseCorrelationId(),
    // the same style as the built-in app.UseAuthentication(), etc.
    public static class CorrelationIdMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
            => app.UseMiddleware<CorrelationIdMiddleware>();
    }
}
