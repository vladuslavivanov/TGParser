using MassTransit;
using Telegram.Bot.Types;
using TGParser.API.Controllers.Commands.Interfaces;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Commands.Implementations.Preset;

public class SetDefaultPreset(IDialogService dialogService, IBus bus) : BaseCommand, ICommand
{
    public string Name => CommandNames.SET_DEFAULT_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        dialogService.SetUserDialog(UserId, DialogType.SetDefaultPreset);

        await bus.Publish(new RequestDialogCommand(update.Message!));

        return;
    }
}
