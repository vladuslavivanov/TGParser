
using Serilog;

namespace TGParser.API.Middleware;

public class ExceptionTo200Middleware
{
    private readonly RequestDelegate _next;

    public ExceptionTo200Middleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            Log.Error(ex, "Exception");
        }
    }
}