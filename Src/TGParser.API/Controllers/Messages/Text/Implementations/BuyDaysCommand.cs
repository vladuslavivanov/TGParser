using MassTransit;
using Telegram.Bot.Types;
using TGParser.API.Controllers.Messages.ChatShared;
using TGParser.API.Controllers.Messages.ChatShared.Interfaces;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Messages.ChatShared.Implementations;

public class BuyDaysCommand(IBus bus, IDialogService dialogService) : BaseTelegramAction, ITextMessage
{
    public string Name => TextMessageNames.BUY_DAYS;

    public async Task Execute(Update update)
    {
        SetContext(update);

        dialogService.SetUserDialog(UserId, DialogType.BuyDays);

        await bus.Publish(new RequestDialogCommand(update.Message!));
    }
}
