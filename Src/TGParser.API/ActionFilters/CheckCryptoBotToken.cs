using Microsoft.AspNetCore.Mvc.Filters;
using TGParser.Configuration;

namespace TGParser.API.ActionFilters;

public class CheckCryptoBotToken : Attribute, IActionFilter
{
    static string CryptoBotToken = ConfigurationStorage.GetCryptoBotSecretToken();

    public void OnActionExecuted(ActionExecutedContext context) { }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;
        if (!httpContext.Request.Headers.TryGetValue("Crypto-Pay-API-Token", out var token))
        {
            httpContext.Response.StatusCode = 404;
            return;
        }

        var expectedToken = CryptoBotToken;

        if (token != expectedToken)
        {
            httpContext.Response.StatusCode = 404;
            return;
        }
    }
}
