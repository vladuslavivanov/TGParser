using Telegram.Bot.Types;

namespace TGParser.API.Controllers.Messages.Interfaces;

public interface IMessage 
{
    Task Execute(Update update);
}
