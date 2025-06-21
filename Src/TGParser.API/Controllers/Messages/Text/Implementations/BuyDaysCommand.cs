using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries;
using TGParser.API.Controllers.Messages.ChatShared.Interfaces;
using TGParser.Configuration;
using TGParser.Configuration.Models;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Messages.ChatShared.Implementations;

public class BuyDaysCommand(ITelegramBotClient client) : BaseTelegramAction, ITextMessage
{
    private static PriceModel price = ConfigurationStorage.GetPrice();
    public string Name => TextMessageNames.BUY_DAYS;

    public async Task Execute(Update update)
    {
        SetContext(update);

        await ShowDays();
    }
    
    async Task ShowDays()
    {
        var keyboard = new InlineKeyboardMarkup(
        [
            [ 
                InlineKeyboardButton.WithCallbackData($"1 день - {price.OneDay}$", $"{CallbackQueryNames.BUY_DAYS}_{(int)BuyDaysStep.BUY_DAYS}_1_{price.OneDay}"), 
                InlineKeyboardButton.WithCallbackData($"7 дней - {price.OneWeek}$", $"{CallbackQueryNames.BUY_DAYS}_{(int)BuyDaysStep.BUY_DAYS}_7_{price.OneWeek}"), 
                InlineKeyboardButton.WithCallbackData($"30 дней - {price.OneMonth}$", $"{CallbackQueryNames.BUY_DAYS}_{(int)BuyDaysStep.BUY_DAYS}_30_{price.OneMonth}")
            ]
        ]);

        await client.SendMessage(
            chatId: ChatId,
            text: "Выберите количество дней для покупки",
            replyMarkup: keyboard);
    }
}
