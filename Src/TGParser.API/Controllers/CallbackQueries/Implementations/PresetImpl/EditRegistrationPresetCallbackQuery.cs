using Telegram.Bot.Types;
using TGParser.API.Controllers.CallbackQueries.Interfaces;

namespace TGParser.API.Controllers.CallbackQueries.Implementations.PresetImpl;

public class EditRegistrationPresetCallbackQuery : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.EDIT_REGISTRATION_PRESET;

    public Task Execute(Update update)
    {
        throw new NotImplementedException();
    }
}
