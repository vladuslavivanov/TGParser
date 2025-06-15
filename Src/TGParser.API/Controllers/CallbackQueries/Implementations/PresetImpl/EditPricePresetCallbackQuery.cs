using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.CallbackQueries.Implementations.PresetImpl;

public class EditPricePresetCallbackQuery(ITelegramBotClient client) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.EDIT_PRICE_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var presetId = CallbackQueryData!.Split('_')[1];

        var propertyToEdit = new InlineKeyboardMarkup(
        [
            [
                InlineKeyboardButton.WithCallbackData("📉 Мин. цена", $"{CallbackQueryNames.SET_PRICE_PRESET}_{PriceType.MinPrice}_{presetId}"),
                InlineKeyboardButton.WithCallbackData("📈 Макс. цена", $"{CallbackQueryNames.SET_PRICE_PRESET}_{PriceType.MaxPrice}_{presetId}")
            ],
            [         
                InlineKeyboardButton.WithCallbackData("◀️ Назад", $"{CallbackQueryNames.SHOW_PRESET}_{presetId}") 
            ]
        ]);

        await client.EditMessageReplyMarkup(ChatId, (int)BotMessageId!, propertyToEdit);
    }
}
