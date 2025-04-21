using MassTransit;
using Telegram.Bot.Types;
using TGParser.API.Controllers.Commands.Interfaces;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Commands.Implementations.Preset;

public class RemovePresetCommand(IBus bus, IDialogService dialogService) : BaseCommand, ICommand
{
    public string Name => CommandNames.REMOVE_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        dialogService.SetUserDialog(UserId, DialogType.RemovePreset);

        await bus.Publish(new RequestDialogCommand(update.Message!));

        return;
    }
}
