using Telegram.Bot.Types;

namespace TGParser.API.Controllers.Commands.Interfaces;

public interface ICommand
{
    string Name { get; }

    Task Execute(Update update);
}
