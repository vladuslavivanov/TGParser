using Telegram.Bot.Types;
using TGParser.API.Controllers.CallbackQueries.Interfaces;

namespace TGParser.API.Controllers.CallbackQueries.Implementations.PresetImpl
{
    public class EditLimitionsPresetCallbackQuery : BaseTelegramAction, ICallbackQuery
    {
        public string Name => CallbackQueryNames.EDIT_LIMITIONS_PRESET;

        public Task Execute(Update update)
        {
            throw new NotImplementedException();
        }
    }
}
