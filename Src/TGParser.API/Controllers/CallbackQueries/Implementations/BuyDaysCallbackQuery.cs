using CryptoPay.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.CallbackQueries.Interfaces;
using TGParser.API.Services.Interfaces;
using TGParser.Configuration;
using TGParser.Configuration.Models;
using TGParser.Core.Enums;
using Update = Telegram.Bot.Types.Update;

namespace TGParser.API.Controllers.CallbackQueries.Implementations;

public class BuyDaysCallbackQuery(ITelegramBotClient client, ICryptoBotService cryptoBotService) : BaseTelegramAction, ICallbackQuery
{
    static string BotLink = ConfigurationStorage.GetChannelName();

    public string Name { get; } = CallbackQueryNames.BUY_DAYS;
    public async Task Execute(Update update)
    {
        SetContext(update);
        
        var splitData = CallbackQueryData!.Split('_').ToList();

        var stepSet = Enum.Parse<BuyDaysStep>(splitData.ElementAtOrDefault(1));
        int.TryParse(splitData.ElementAtOrDefault(2), out var quantityDays);
        double.TryParse(splitData.ElementAtOrDefault(3), out var price);

        switch (stepSet)
        {
            case BuyDaysStep.BUY_DAYS:
                await BuyDays(quantityDays, price);
                break;
        }
    }
    
    async Task BuyDays(int quantityDays, double price)
    {
        var link = await cryptoBotService.CreateInvoiceLink(
            CurrencyTypes.crypto,
            "USDT",
            price,
            $"{quantityDays}_{price}_{UserId}",
            "Дни начислены!", 
            PaidButtonNames.openBot,
            $"https://t.me/{BotLink}",
            quantityDays.ToString(),
            300
        );

        await client.EditMessageText(ChatId, (int)BotMessageId!, "Для оплаты перейдите по ссылке", 
            replyMarkup: InlineKeyboardButton.WithUrl("Оплатить", link));
    }
}