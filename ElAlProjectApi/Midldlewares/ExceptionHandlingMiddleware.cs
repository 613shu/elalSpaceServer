using Microsoft.EntityFrameworkCore;

using System.Net;
using System.Text.Json;

namespace SukotSystemApi.Middlewares
{
    // The single place in the whole server that turns "something went wrong"
    // into an HTTP response. Every exception that reaches here was NOT handled
    // closer to where it happened, so this is the last line of defense - it
    // must never let a raw .NET exception/stack trace leak to the client.
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            // Most specific exception first: this is the one Part C is about.
            // EF Core throws this when the WHERE clause built from the
            // concurrency token no longer matches any row - i.e. someone
            // else already saved a change to the same entity.
            catch (DbUpdateConcurrencyException ex)
            {
                await WriteErrorAsync(
                    context, ex,
                    HttpStatusCode.Conflict,
                    "The resource was changed by another user. Please refresh and try again.",
                    LogLevel.Warning); // Part D: every conflict is logged at Warning, not Error.
            }
            // Also 409, but a different flavor of conflict: a business rule was violated
            // against the current state of the data (e.g. a duplicate phone on the Rabbi
            // roster, or claiming/completing an Order that is no longer in the right status)
            // rather than a lost optimistic-concurrency race. Thrown deliberately by the
            // Service layer - not a bug.
         
            // Business-level "not found" - thrown deliberately by the Service
            // layer (see CustomerService.GetCustomerById), not a real failure.
            catch (KeyNotFoundException ex)
            {
                await WriteErrorAsync(context, ex, HttpStatusCode.NotFound, ex.Message, LogLevel.Warning);
            }
            // Wrong credentials / no permission - also an expected, handled
            // outcome, not a bug in the server.
            catch (UnauthorizedAccessException ex)
            {
                await WriteErrorAsync(context, ex, HttpStatusCode.Unauthorized, ex.Message, LogLevel.Warning);
            }
            // Anything else is, by definition, a bug or an unexpected failure -
            // this is the only branch that logs at Error level.
            catch (Exception ex)
            {
                await WriteErrorAsync(
                    context, ex,
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred. Please try again later.",
                    LogLevel.Error);
            }
        }

        private async Task WriteErrorAsync(
            HttpContext context,
            Exception ex,
            HttpStatusCode statusCode,
            string clientMessage,
            LogLevel level)
        {
            var correlationId = context.Items.TryGetValue("CorrelationId", out var id)
                ? id?.ToString() ?? string.Empty
                : string.Empty;

            // Never log the exception's own message at Error unless it really
            // is one - see the level chosen by each catch block above. The
            // exception object itself is still passed to ILogger so the full
            // stack trace lands in the log, even though the client never sees it.
            _logger.Log(level, ex, "Request failed with {StatusCode} [{CorrelationId}]", (int)statusCode, correlationId);

            // If the response has already started streaming (e.g. a large
            // payload was partially written), we physically cannot change
            // its status code or body anymore - so there is nothing left to do
            // but stop; the client will just see a truncated/broken response.
            if (context.Response.HasStarted)
                return;

            // NOTE: deliberately NOT calling context.Response.Clear() here.
            // Clear() wipes every header already set on the response - including
            // the Access-Control-Allow-Origin header the CORS middleware adds
            // earlier in the pipeline (it runs before this middleware's catch
            // block executes, since it's registered after this one and calls
            // next() before any exception bubbles back up here). Clearing the
            // response was stripping that header on every single error reply
            // (400/401/403/404/409/500), which made the browser report a CORS
            // failure on any failed request instead of showing the real error.
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var body = new ApiErrorResponse
            {
                StatusCode = (int)statusCode,
                Message = clientMessage,
                CorrelationId = correlationId
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(body));
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
            => app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
