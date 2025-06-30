
using Serilog;
using TGParser.API.Attributes;

namespace TGParser.API.Middleware;

public class ExceptionTo200Middleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint?.Metadata.GetMetadata<Skip200MiddlewareAttribute>() == null)
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";

                Log.Error(ex, "Exception");
            }
        }
    }
}