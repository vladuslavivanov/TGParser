using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.CallbackQueries.Implementations.PresetImpl;

public class SetPricePresetCallbackQuery(ITelegramBotClient client) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.SET_PRICE_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var splitData = CallbackQueryData!.Split('_');

        var priceType = Enum.Parse<PriceType>(splitData[1]);
        var presetId = int.Parse(splitData[2]);

        var keyboard = new InlineKeyboardMarkup();

        if (priceType == PriceType.MinPrice)
        {
            keyboard.AddNewRow(InlineKeyboardButton.WithCallbackData("0", $"{CallbackQueryNames.SHOW_PRESET}_{presetId}"));
        }

        var basePrice = new List<int>() { 1, 2, 5 };

        foreach (var item in basePrice)
        {
            keyboard.AddNewRow(
            [
                InlineKeyboardButton.WithCallbackData((item * 10).ToString(), $"{CallbackQueryNames.SHOW_PRESET}_{presetId}"),
                    InlineKeyboardButton.WithCallbackData((item * 100).ToString(), $"{CallbackQueryNames.SHOW_PRESET}_{presetId}"),
                    InlineKeyboardButton.WithCallbackData((item * 1000).ToString(), $"{CallbackQueryNames.SHOW_PRESET}_{presetId}"),
                ]);
        }

        if (priceType == PriceType.MaxPrice)
        {
            keyboard.AddNewRow(InlineKeyboardButton.WithCallbackData("ထ", $"{CallbackQueryNames.SHOW_PRESET}_{presetId}"));
        }


        keyboard.AddNewRow(InlineKeyboardButton.WithCallbackData("◀️ Назад", $"{CallbackQueryNames.SHOW_PRESET}_{presetId}"));

        await client.EditMessageReplyMarkup(ChatId, (int)BotMessageId!, keyboard);
    }
}
