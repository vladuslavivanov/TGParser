using Telegram.Bot.Types;

namespace TGParser.Services.Implementations;

public interface IMessageHandler
{
    Task HandleMessageAsync(Update update);
}
