using MassTransit;
using Telegram.Bot.Types;
using TGParser.API.Controllers.Commands.Interfaces;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Commands.Implementations.Proxy;

public class RemoveProxyCommand(IBus bus, IDialogService dialogService) : BaseCommand, ICommand
{
    public string Name => CommandNames.REMOVE_PROXY;

    public async Task Execute(Update update)
    {
        SetContext(update);

        dialogService.SetUserDialog(UserId, DialogType.RemoveProxy);

        await bus.Publish(new RequestDialogCommand(update.Message!));
    }
}
