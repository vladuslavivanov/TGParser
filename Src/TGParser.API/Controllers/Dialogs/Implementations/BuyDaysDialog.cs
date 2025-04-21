using MassTransit;
using Telegram.Bot;
using Telegram.Bot.Types;
using TGParser.API.Controllers.Dialogs.Interfaces;
using TGParser.API.Services.Interfaces;
using TGParser.Configuration.Models;
using TGParser.Core.Enums;
using TGParser.Configuration;
using Telegram.Bot.Types.ReplyMarkups;
using TGParser.API.Controllers.Commands;
using CryptoPay.Types;
using TGParser.API.Controllers.Dialogs.Contexts;

namespace TGParser.API.Controllers.Dialogs.Implementations;

public class BuyDaysDialog(ITelegramBotClient client, IBus bus, 
    IDialogService dialogService, ICryptoBotService cryptoBotService) 
    : BaseDialog<BuyDaysContext>(bus, dialogService, client), IDialog
{
    static string BotLink = ConfigurationStorage.GetChannelName();

    static PriceModel price = ConfigurationStorage.GetPrice();

    public DialogType DialogType => DialogType.BuyDays;

    public async Task Execute(Message message)
    {
        SetContext(new() { Message = message });

        _dialogContexts.TryGetValue(UserId, out var dialogContext);

        // Начало диалога.
        if (dialogContext == default)
        {
            _dialogContexts[UserId] = new();
            await SendMenu();
            return;
        }

        // Пользователь выбрал количество дней.
        if (dialogContext.DialogState == DialogState.FirstStep)
        {
            if (await TryHandleUserLeaveAsync(nextCommandName: CommandNames.PROFILE)) return;

            if (!ValidateUserWrite(message.Text!, out var selectedPrice))
            {
                await SendMenu();
                return;
            }

            // Стоимость 
            dialogContext.Price = (int)selectedPrice!;

            // Количество выбранных дней
            dialogContext.QuantityDays = int.Parse(message.Text!.Split(' ')[0]);

            dialogContext.DialogState = DialogState.SecondStep;

            var link = await cryptoBotService.CreateInvoiceLink(
                CurrencyTypes.fiat,
                "RUB",
                dialogContext.Price,
                "", "", PaidButtonNames.openBot,
                $"https://t.me/{BotLink}",
                dialogContext.QuantityDays.ToString(),
                600
                );

            dialogContext.LinkToPay = link;

            await client.SendMessage(ChatId,
                "Для оплаты перейдите по ссылке\n" +
                $"{link}");

            dialogContext.DialogState = DialogState.SecondStep;
            return;
        }

        // Пользователь выбрал способ оплаты.
        if (dialogContext.DialogState == DialogState.SecondStep)
        {
            
        }
    }

    async Task SendMenu()
    {
        var keyboard = new ReplyKeyboardMarkup(
        [
            [$"1 день = {price.OneDay} р", $"7 дней = {price.OneWeek} р", $"30 дней = {price.OneMonth} р"],
            [EditingNames.LEAVE]
        ])
        {
            ResizeKeyboard = true,
        };

        await client.SendMessage(
            chatId: ChatId,
            text: "Выберите количество дней доступа",
            replyMarkup: keyboard);
    }

    bool ValidateUserWrite(string message, out int? selectedPrice)
    {
        if (message == $"1 день = {price.OneDay} р")
        {
            selectedPrice = price.OneDay;
            return true;
        }
        if (message == $"7 дней = {price.OneWeek} р")
        {
            selectedPrice = price.OneWeek;
            return true;
        }
        if (message == $"30 дней = {price.OneMonth} р")
        {
            selectedPrice = price.OneMonth;
            return true;
        }
        selectedPrice = null;
        return false;
    }

    bool ValidatePayMethod(string message)
    {
        var currencies = Enum.GetNames(typeof(CurrencyEnum)).ToList();
        return currencies.Contains(message);
    }
}
