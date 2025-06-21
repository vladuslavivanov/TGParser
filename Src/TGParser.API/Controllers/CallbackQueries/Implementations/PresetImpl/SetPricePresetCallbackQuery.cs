using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries.Interfaces;
using TGParser.API.Controllers.Helpers;
using TGParser.BLL.Interfaces;
using TGParser.Core.Enums;
using TGParser.Core.Enums.Presets;

namespace TGParser.API.Controllers.CallbackQueries.Implementations.PresetImpl;

public class SetPricePresetCallbackQuery(ITelegramBotClient client, IPresetManager presetManager) : BaseTelegramAction, ICallbackQuery
{
    public string Name => CallbackQueryNames.SET_PRICE_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var splitData = CallbackQueryData!.Split('_').ToList();
        
        var presetId = int.Parse(splitData[1]);
        var stepSet = Enum.Parse<SetPriceStep>(splitData.ElementAtOrDefault(2) ?? SetPriceStep.SELECT_TYPE_PRICE.ToString());
        Enum.TryParse<PriceType>(splitData.ElementAtOrDefault(3), out var priceType);
        int.TryParse(splitData.ElementAtOrDefault(4), out var price);
        
        switch (stepSet)
        {
            case SetPriceStep.SELECT_TYPE_PRICE:
                await SelectTypePriceStep(presetId);
                break;
            case SetPriceStep.SELECT_PRICE:
                await SelectPriceStep(presetId, priceType);
                break;
            case SetPriceStep.SET_PRICE:
                await presetManager.SetPrice(UserId, presetId, priceType, price);
                await UpdateMessageHelper.UpdateUserPreset(client, presetManager, UserId, presetId, (int)BotMessageId!, Message!.ReplyMarkup);
                await client.AnswerCallbackQuery(CallbackQueryId!, "✅ Готово");
                break;
        }
    }

    async Task SelectTypePriceStep(int presetId)
    {
        var propertyToEdit = new InlineKeyboardMarkup(
        [
            [
                InlineKeyboardButton.WithCallbackData("📉 Мин. цена", $"{CallbackQueryNames.SET_PRICE_PRESET}_{presetId}_{(int)SetPriceStep.SELECT_PRICE}_{(int)PriceType.MinPrice}"),
                InlineKeyboardButton.WithCallbackData("📈 Макс. цена", $"{CallbackQueryNames.SET_PRICE_PRESET}_{presetId}_{(int)SetPriceStep.SELECT_PRICE}_{(int)PriceType.MaxPrice}")
            ],
            [         
                InlineKeyboardButton.WithCallbackData("◀️ Назад", $"{CallbackQueryNames.EDIT_PRESET}_{presetId}") 
            ]
        ]);

        await client.EditMessageReplyMarkup(ChatId, (int)BotMessageId!, propertyToEdit);
    }

    async Task SelectPriceStep(int presetId, PriceType priceType)
    {
        var keyboard = new InlineKeyboardMarkup();
        
        if (priceType == PriceType.MinPrice)
        {
            keyboard.AddNewRow(InlineKeyboardButton.WithCallbackData("0", $"{CallbackQueryNames.SET_PRICE_PRESET}_{presetId}_{(int)SetPriceStep.SET_PRICE}_{priceType}_0"));
        }

        var basePrice = new List<int> { 1, 2, 5 };

        foreach (var item in basePrice)
        {
            keyboard.AddNewRow(
            [
                InlineKeyboardButton.WithCallbackData((item * 10).ToString(), $"{CallbackQueryNames.SET_PRICE_PRESET}_{presetId}_{(int)SetPriceStep.SET_PRICE}_{priceType}_{item * 10}"),
                InlineKeyboardButton.WithCallbackData((item * 100).ToString(), $"{CallbackQueryNames.SET_PRICE_PRESET}_{presetId}_{(int)SetPriceStep.SET_PRICE}_{priceType}_{item * 100}"),
                InlineKeyboardButton.WithCallbackData((item * 1000).ToString(), $"{CallbackQueryNames.SET_PRICE_PRESET}_{presetId}_{(int)SetPriceStep.SET_PRICE}_{priceType}_{item * 1000}"),
            ]);
        }

        if (priceType == PriceType.MaxPrice)
        {
            keyboard.AddNewRow(InlineKeyboardButton.WithCallbackData("Максимум", $"{CallbackQueryNames.SET_PRICE_PRESET}_{presetId}_{(int)SetPriceStep.SET_PRICE}_{priceType}_{int.MaxValue}"));
        }

        keyboard.AddNewRow(InlineKeyboardButton.WithCallbackData("◀️ Назад", $"{CallbackQueryNames.EDIT_PRESET}_{presetId}"));

        await client.EditMessageReplyMarkup(ChatId, (int)BotMessageId!, keyboard);
    }
}
