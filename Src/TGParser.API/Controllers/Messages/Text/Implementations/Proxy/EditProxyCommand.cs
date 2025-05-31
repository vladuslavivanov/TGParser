using MassTransit;
using Telegram.Bot.Types;
using TGParser.API.Controllers.Messages.ChatShared;
using TGParser.API.Controllers.Messages.ChatShared.Interfaces;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Messages.ChatShared.Implementations.Proxy;

public class EditProxyCommand(IBus bus, IDialogService dialogService) : BaseTelegramAction, ITextMessage
{
    public string Name => TextMessageNames.EDIT_PROXY;

    public async Task Execute(Update update)
    {
        SetContext(update);

        dialogService.SetUserDialog(UserId, DialogType.EditingProxy);

        await bus.Publish(new RequestDialogCommand(update.Message!));
    }
}
