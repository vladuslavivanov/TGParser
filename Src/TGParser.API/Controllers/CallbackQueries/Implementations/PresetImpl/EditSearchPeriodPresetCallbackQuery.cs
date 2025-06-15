using Telegram.Bot;
using Telegram.Bot.Types;
using TGParser.API.Controllers.CallbackQueries.Interfaces;

namespace TGParser.API.Controllers.CallbackQueries.Implementations.PresetImpl;

public class EditSearchPeriodPresetCallbackQuery(ITelegramBotClient client) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.EDIT_SEARCH_PERIOD_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);
        var presetId = CallbackQueryData!.Split('_')[1];
        var keyboardMarkup = CallbackQueryHelper.ConfigurePeriodSearchKeyboard(int.Parse(presetId));

        await client.EditMessageReplyMarkup(ChatId, (int)BotMessageId!, keyboardMarkup);
    }
}
