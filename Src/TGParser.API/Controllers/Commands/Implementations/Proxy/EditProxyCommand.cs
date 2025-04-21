using MassTransit;
using Telegram.Bot.Types;
using TGParser.API.Controllers.Commands.Interfaces;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Commands.Implementations.Proxy;

public class EditProxyCommand(IBus bus, IDialogService dialogService) : BaseCommand, ICommand
{
    public string Name => CommandNames.EDIT_PROXY;

    public async Task Execute(Update update)
    {
        SetContext(update);

        dialogService.SetUserDialog(UserId, DialogType.EditingProxy);

        await bus.Publish(new RequestDialogCommand(update.Message!));
    }
}
