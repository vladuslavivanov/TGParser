using MassTransit;
using Telegram.Bot;
using Telegram.Bot.Types;
using TGParser.API.Controllers.Commands.Interfaces;
using TGParser.API.MassTransit.Requsted;
using TGParser.API.Services.Interfaces;
using TGParser.BLL.Interfaces;
using TGParser.Core.Consts;
using TGParser.Core.Enums;

namespace TGParser.API.Controllers.Commands.Implementations.Preset;

public class AddPresetCommand(ITelegramBotClient client,
    IPresetManager presetManager, IBus bus, IDialogService dialogService) : BaseCommand, ICommand
{
    public string Name => CommandNames.ADD_PRESET;

    public async Task Execute(Update update)
    {
        SetContext(update);

        var quantityPresets = await presetManager.GetQuantityPresetsAsync(UserId);

        if (quantityPresets >= BotConstants.MAX_PRESETS)
        {
            await client.SendMessage(
                chatId: ChatId,
                text: $"Доступно максимум {BotConstants.MAX_PRESETS} пересетов❗"
                );
            return;
        }

        dialogService.SetUserDialog(UserId, DialogType.AddPreset);

        await bus.Publish(new RequestDialogCommand(update.Message!));
    }
}
