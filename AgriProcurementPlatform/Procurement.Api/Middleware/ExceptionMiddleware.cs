using System.Net;
using System.Text.Json;

namespace Procurement.Api.Middleware;

public sealed class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ArgumentException ex)
        {
            await WriteProblem(context, HttpStatusCode.BadRequest, "Bad Request", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteProblem(context, HttpStatusCode.Conflict, "Conflict", ex.Message);
        }
        catch (Exception ex)
        {
            await WriteProblem(context, HttpStatusCode.InternalServerError, "Server Error", ex.Message);
        }
    }

    private static async Task WriteProblem(HttpContext ctx, HttpStatusCode status, string title, string detail)
    {
        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = (int)status;

        var payload = new
        {
            type = "about:blank",
            title,
            status = (int)status,
            detail,
            traceId = ctx.TraceIdentifier
        };

        await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
