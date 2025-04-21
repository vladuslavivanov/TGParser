using Microsoft.AspNetCore.Mvc.Filters;
using TGParser.Configuration;

namespace TGParser.API.ActionFilters;

public class CheckTelegramToken : Attribute, IActionFilter
{
    static string TelegramToken = ConfigurationStorage.GetTelegramSecretToken();

    public void OnActionExecuted(ActionExecutedContext context) { }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;
        if (!httpContext.Request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out var token))
        {
            httpContext.Response.StatusCode = 404;
            return;
        }

        var expectedToken = TelegramToken;

        if (token != expectedToken)
        {
            httpContext.Response.StatusCode = 404;
            return;
        }
    }
}
