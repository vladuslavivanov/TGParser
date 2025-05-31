using MassTransit;
using Telegram.Bot.Types;
using TGParser.API.Controllers.Messages.ChatShared;
using TGParser.API.Controllers.Messages.ChatShared.Interfaces;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Messages.ChatShared.Implementations.Preset;

public class SetDefaultPreset(IDialogService dialogService, IBus bus) : BaseTelegramAction, ITextMessage
{
    public string Name => TextMessageNames.SET_DEFAULT_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        dialogService.SetUserDialog(UserId, DialogType.SetDefaultPreset);

        await bus.Publish(new RequestDialogCommand(update.Message!));

        return;
    }
}
