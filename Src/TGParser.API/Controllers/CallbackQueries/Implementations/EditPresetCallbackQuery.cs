using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries.Interfaces;
using TGParser.Core.Enums.Presets;

namespace TGParser.API.Controllers.CallbackQueries.Implementations;

public class EditPresetCallbackQuery(ITelegramBotClient client) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.EDIT_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var presetId = CallbackQueryData!.Split('_')[1];

        var propertyToEdit = new InlineKeyboardMarkup(
        [
            [ InlineKeyboardButton.WithCallbackData("🗓 Редактировать период поиска", $"{CallbackQueryNames.SET_SEARCH_PERIOD_PRESET}_{presetId}_{SetSearchPeriodStep.ShowVariously}") ],
            [ InlineKeyboardButton.WithCallbackData("💰 Редактировать параметры цены", $"{CallbackQueryNames.SET_PRICE_PRESET}_{presetId}") ],
            [ InlineKeyboardButton.WithCallbackData("📅 Редактировать период регистрации продавца", $"{CallbackQueryNames.SET_REGISTRATION_PRESET}_{presetId}") ],
            [ InlineKeyboardButton.WithCallbackData("📦 Редактировать ограничения по товарам и спискам продавца", $"{CallbackQueryNames.SET_LIMITATIONS_PRESET}_{presetId}") ], 
            [ InlineKeyboardButton.WithCallbackData("◀️ Назад", $"{CallbackQueryNames.SHOW_PRESET}_{presetId}") ],
        ]);

        await client.EditMessageReplyMarkup(ChatId, (int)BotMessageId!, propertyToEdit);
    }
}
