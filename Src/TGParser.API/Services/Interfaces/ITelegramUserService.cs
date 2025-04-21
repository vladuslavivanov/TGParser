using Telegram.Bot.Types;

namespace TGParser.API.Services.Interfaces;

public interface ITelegramUserService
{
    Task AddUserIfNotExists(Message message);

    Task<bool> IsUserSubscribed(Message message);
}
