using Telegram.Bot.Types;

namespace TGParser.API.Controllers.CallbackQueries.Interfaces;

public interface ICallbackQuery
{
    string Name { get; }

    Task Execute(Update update);
}
